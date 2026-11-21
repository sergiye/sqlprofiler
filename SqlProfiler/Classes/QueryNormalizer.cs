using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlProfiler {
  /// <summary>
  /// Makes the same query with different values look the same: numbers and strings become "?", comments are dropped, white space is
  /// collapsed. The text of sp_executesql is normalized itself, so that its calls are not all one query.
  /// </summary>
  internal static class QueryNormalizer {
    private static readonly Regex ValueList = new(@"\?(\s*,\s*\?)+", RegexOptions.Compiled);

    public static string Normalize(string sql) {
      if (string.IsNullOrWhiteSpace(sql)) return "";

      var tokens = Merge(new YukonLexer().Tokenize(sql));
      var sb = new StringBuilder();
      var space = false;
      string previousWord = null;

      for (var i = 0; i < tokens.Count; i++) {
        var token = tokens[i];
        switch (token.Kind) {
          case YukonLexer.TokenKind.tkSpace:
            if (!token.InsideBlockComment) space = true;
            continue;
          case YukonLexer.TokenKind.tkComment:
            continue;
        }

        string text;
        if (token.Kind == YukonLexer.TokenKind.tkNumber) {
          text = "?";
        }
        else if (token.Kind == YukonLexer.TokenKind.tkString) {
          text = IsAfterExecuteSql(previousWord) ? ExecuteSqlText(token.Text) : "?";
        }
        else if (IsUnicodePrefix(tokens, i)) {
          // N'text': the prefix goes together with the string
          continue;
        }
        else {
          text = token.Text;
          previousWord = text;
        }

        if (space && sb.Length > 0) sb.Append(' ');
        sb.Append(text);
        space = false;
      }

      return ValueList.Replace(sb.ToString().Trim(), "?, ...");
    }

    /// <summary>Adjacent string tokens are one literal: a line break or a doubled quote inside it splits it into several tokens.</summary>
    internal static List<SqlToken> Merge(List<SqlToken> tokens) {
      var result = new List<SqlToken>(tokens.Count);
      foreach (var token in tokens) {
        if (token.Kind == YukonLexer.TokenKind.tkString && result.Count > 0 && result[result.Count - 1].Kind == YukonLexer.TokenKind.tkString)
          result[result.Count - 1] = new SqlToken(token.Kind, result[result.Count - 1].Text + token.Text, false);
        else
          result.Add(token);
      }

      return result;
    }

    private static bool IsUnicodePrefix(List<SqlToken> tokens, int index) {
      var text = tokens[index].Text;
      return (text == "N" || text == "n") && index + 1 < tokens.Count && tokens[index + 1].Kind == YukonLexer.TokenKind.tkString;
    }

    private static bool IsAfterExecuteSql(string previousWord) {
      return string.Equals(previousWord, "sp_executesql", StringComparison.OrdinalIgnoreCase);
    }

    private static string ExecuteSqlText(string literal) {
      if (literal.Length < 2 || literal[0] != '\'' || literal[literal.Length - 1] != '\'') return "?";
      var inner = literal.Substring(1, literal.Length - 2).Replace("''", "'");
      return "N'" + Normalize(inner).Replace("'", "''") + "'";
    }
  }
}
