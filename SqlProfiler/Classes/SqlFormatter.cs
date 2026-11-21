using System;
using System.Collections.Generic;
using System.Text;

namespace SqlProfiler {
  internal sealed class SqlToken {
    public SqlToken(YukonLexer.TokenKind kind, string text, bool insideBlockComment) {
      Kind = kind;
      Text = text;
      InsideBlockComment = insideBlockComment;
    }

    public YukonLexer.TokenKind Kind { get; }
    public string Text { get; }

    /// <summary>The token (a line break) is a part of a comment that spans several lines.</summary>
    public bool InsideBlockComment { get; }
  }

  /// <summary>Puts the main clauses of a query on separate lines. Only the white space outside of strings and comments is changed.</summary>
  internal static class SqlFormatter {
    private const int IndentSize = 4;

    private static readonly HashSet<string> ClauseKeywords = new(StringComparer.OrdinalIgnoreCase) {
      "SELECT", "FROM", "WHERE", "HAVING", "UNION", "EXCEPT", "INTERSECT", "INSERT", "UPDATE", "DELETE", "SET", "VALUES",
      "DECLARE", "MERGE", "GROUP", "ORDER", "EXEC", "EXECUTE"
    };

    // the arguments of a call (exec) are listed one per line too
    private static readonly HashSet<string> ListClauses = new(StringComparer.OrdinalIgnoreCase) {"SELECT", "SET", "GROUP", "ORDER", "EXEC", "EXECUTE"};
    private static readonly HashSet<string> ConditionClauses = new(StringComparer.OrdinalIgnoreCase) {"WHERE", "HAVING", "ON"};
    private static readonly HashSet<string> JoinPrefixes = new(StringComparer.OrdinalIgnoreCase) {"INNER", "LEFT", "RIGHT", "FULL", "CROSS"};
    private static readonly HashSet<string> JoinParts = new(StringComparer.OrdinalIgnoreCase) {"INNER", "LEFT", "RIGHT", "FULL", "CROSS", "OUTER", "JOIN"};

    public static string Format(string sql) {
      if (string.IsNullOrWhiteSpace(sql)) return sql ?? "";

      var tokens = QueryNormalizer.Merge(new YukonLexer().Tokenize(sql));
      var sb = new StringBuilder();
      var clauses = new Stack<string>();
      clauses.Push("");
      var lineStart = true;
      var forceNewline = false;
      var spaceBefore = false;
      var inBetween = false;
      string previous = null;

      for (var i = 0; i < tokens.Count; i++) {
        var token = tokens[i];
        if (token.Kind == YukonLexer.TokenKind.tkSpace) {
          if (token.InsideBlockComment) {
            sb.Append(token.Text);
            lineStart = false;
          }
          else {
            spaceBefore = true;
          }

          continue;
        }

        var text = token.Text;
        var isWord = token.Kind != YukonLexer.TokenKind.tkString && token.Kind != YukonLexer.TokenKind.tkComment;
        var isUnicodePrefix = (text == "N" || text == "n") && i + 1 < tokens.Count && tokens[i + 1].Kind == YukonLexer.TokenKind.tkString;
        // the query that sp_executesql runs is written as a string: the text of this string is formatted too
        if (token.Kind == YukonLexer.TokenKind.tkString && string.Equals(previous, "sp_executesql", StringComparison.OrdinalIgnoreCase))
          text = FormatExecuteSqlText(text);
        var level = clauses.Count - 1;
        var newline = forceNewline;
        var indent = level * IndentSize;
        forceNewline = false;

        if (isWord) {
          if (ClauseKeywords.Contains(text) && !(previous is not null && previous.Equals("DELETE", StringComparison.OrdinalIgnoreCase) && text.Equals("FROM", StringComparison.OrdinalIgnoreCase))) {
            newline = true;
            SetClause(clauses, text.ToUpperInvariant());
          }
          else if (IsJoinStart(tokens, i, previous)) {
            newline = true;
            SetClause(clauses, "JOIN");
          }
          else if (text.Equals("ON", StringComparison.OrdinalIgnoreCase)) {
            SetClause(clauses, "ON");
          }
          else if (text.Equals("BETWEEN", StringComparison.OrdinalIgnoreCase)) {
            inBetween = true;
          }
          else if (inBetween && text.Equals("AND", StringComparison.OrdinalIgnoreCase)) {
            // the AND of "BETWEEN x AND y" is a part of the condition
            inBetween = false;
          }
          else if ((text.Equals("AND", StringComparison.OrdinalIgnoreCase) || text.Equals("OR", StringComparison.OrdinalIgnoreCase))
                   && ConditionClauses.Contains(clauses.Peek())) {
            newline = true;
            indent += IndentSize;
          }
        }

        if (newline && sb.Length > 0) {
          TrimEnd(sb);
          sb.Append("\r\n").Append(' ', indent);
          lineStart = true;
        }
        else if (spaceBefore && !lineStart && sb.Length > 0) {
          sb.Append(' ');
        }

        sb.Append(text);
        lineStart = false;
        spaceBefore = false;

        if (text == "(") {
          clauses.Push("(");
        }
        else if (text == ")" && clauses.Count > 1) {
          clauses.Pop();
        }
        else if (text == "," && ListClauses.Contains(clauses.Peek())) {
          // the next item of the list is indented under its clause
          sb.Append("\r\n").Append(' ', level * IndentSize + IndentSize);
          lineStart = true;
        }
        else if (text == ";") {
          forceNewline = true;
          clauses.Clear();
          clauses.Push("");
        }

        if (token.Kind == YukonLexer.TokenKind.tkComment && text.StartsWith("--", StringComparison.Ordinal))
          forceNewline = true;

        if (isWord && !isUnicodePrefix) previous = text;
      }

      return sb.ToString().Trim();
    }

    private static string FormatExecuteSqlText(string literal) {
      if (literal.Length < 2 || literal[0] != '\'' || literal[literal.Length - 1] != '\'') return literal;
      var inner = literal.Substring(1, literal.Length - 2).Replace("''", "'");
      if (string.IsNullOrWhiteSpace(inner)) return literal;
      return "'\r\n" + Format(inner).Replace("'", "''") + "'";
    }

    private static void SetClause(Stack<string> clauses, string clause) {
      clauses.Pop();
      clauses.Push(clause);
    }

    private static void TrimEnd(StringBuilder sb) {
      while (sb.Length > 0 && sb[sb.Length - 1] == ' ')
        sb.Length--;
    }

    private static bool IsJoinStart(List<SqlToken> tokens, int index, string previous) {
      var text = tokens[index].Text;
      if (JoinPrefixes.Contains(text)) {
        var next = NextWord(tokens, index);
        return next is not null && JoinParts.Contains(next);
      }

      return text.Equals("JOIN", StringComparison.OrdinalIgnoreCase)
             && (previous is null || !JoinParts.Contains(previous));
    }

    private static string NextWord(List<SqlToken> tokens, int index) {
      for (var i = index + 1; i < tokens.Count; i++) {
        if (tokens[i].Kind != YukonLexer.TokenKind.tkSpace)
          return tokens[i].Text;
      }

      return null;
    }
  }
}
