using System;
using System.Drawing;
using System.Text;

namespace SqlProfiler {
  public class YukonLexer {
    public enum TokenKind {
      tkComment,
      tkDatatype,
      tkFunction,
      tkIdentifier,
      tkKey,
      tkNull,
      tkNumber,
      tkSpace,
      tkString,
      tkSymbol,
      tkUnknown,
      tkVariable,
      tkGreyKeyword,
      tkFuKeyword
    }

    private enum SqlRange {
      rsUnknown,
      rsComment,
      rsString
    }

    private readonly Sqltokens _tokens = new Sqltokens();

    const string IdentifierStr = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_#$";
    private readonly char[] _identifiersArray = IdentifierStr.ToCharArray();
    const string HexDigits = "1234567890abcdefABCDEF";
    const string NumberStr = "1234567890.-";
    private int _stringLen;
    private int _tokenPos;
    private string _token = "";
    private TokenKind _tokenId;
    private string _line;
    private int _run;

    private TokenKind TokenId {
      get { return _tokenId; }
    }

    private string Token {
      get {
        /*int len = _run - _tokenPos; return _line.Substring(_tokenPos, len);*/
        return _token;
      }
    }

    private SqlRange Range { get; set; }

    private char GetChar(int idx) {
      return idx >= _line.Length ? '\x00' : _line[idx];
    }

    public YukonLexer() {
      Array.Sort(_identifiersArray);
    }

    #region Colors

    public Color DefaultColor { get; set; } = Color.Black;
    public Color KeywordColor { get; set; } = Color.Blue;
    public Color GreyKeywordColor { get; set; } = Color.Gray;
    public Color FunctionColor { get; set; } = Color.Fuchsia;
    public Color ValueColor { get; set; } = Color.Red;
    public Color CommentColor { get; set; } = Color.DarkGreen;

    #endregion

    /// <summary>Splits the text into tokens; the white space between them is returned as tokens too.</summary>
    internal System.Collections.Generic.List<SqlToken> Tokenize(string sql) {
      var result = new System.Collections.Generic.List<SqlToken>();
      Range = SqlRange.rsUnknown;
      _line = sql;
      _run = 0;
      while (true) {
        var insideBlockComment = Range == SqlRange.rsComment;
        Next();
        if (_tokenId == TokenKind.tkNull) break;
        result.Add(new SqlToken(_tokenId, _token, insideBlockComment));
      }

      return result;
    }

    public void FillRichEdit(System.Windows.Forms.RichTextBox rich, string value) {
      rich.Text = "";
      Line = value;

      RTFBuilder sb = new RTFBuilder {BackColor = rich.BackColor};
      while (TokenId != TokenKind.tkNull) {
        switch (TokenId) {
          case TokenKind.tkKey:
            sb.ForeColor = KeywordColor;
            break;
          case TokenKind.tkFunction:
            sb.ForeColor = FunctionColor;
            break;
          case TokenKind.tkGreyKeyword:
            sb.ForeColor = GreyKeywordColor;
            break;
          case TokenKind.tkFuKeyword:
            sb.ForeColor = FunctionColor;
            break;
          case TokenKind.tkDatatype:
            sb.ForeColor = KeywordColor;
            break;
          case TokenKind.tkNumber:
            sb.ForeColor = ValueColor;
            break;
          case TokenKind.tkString:
            sb.ForeColor = ValueColor;
            break;
          case TokenKind.tkComment:
            sb.ForeColor = CommentColor;
            break;
          default:
            sb.ForeColor = DefaultColor;
            break;
        }

        if (Token == Environment.NewLine || Token == "\r" || Token == "\n") {
          sb.AppendLine();
        }
        else {
          sb.Append(Token);
        }

        Next();
      }

      rich.Rtf = sb.ToString();
    }

    private string Line {
      set {
        Range = SqlRange.rsUnknown;
        _line = value;
        _run = 0;
        Next();
      }
    }

    private void NullProc() {
      _tokenId = TokenKind.tkNull;
    }

    // ReSharper disable InconsistentNaming
    private void LFProc() {
      _tokenId = TokenKind.tkSpace;
      _run++;
    }

    private void CRProc() {
      _tokenId = TokenKind.tkSpace;
      _run++;
      if (GetChar(_run) == '\x0A') _run++;
    }
    // ReSharper restore InconsistentNaming

    private void AnsiCProc() {
      switch (GetChar(_run)) {
        case '\x00':
          NullProc();
          break;
        case '\x0A':
          LFProc();
          break;
        case '\x0D':
          CRProc();
          break;

        default: {
          _tokenId = TokenKind.tkComment;
          char c;
          do {
            if (GetChar(_run) == '*' && GetChar(_run + 1) == '/') {
              Range = SqlRange.rsUnknown;
              _run += 2;
              break;
            }

            _run++;
            c = GetChar(_run);
          } while (!(c == '\x00' || c == '\x0A' || c == '\x0D'));

          break;
        }
      }
    }

    private void AsciiCharProc() {
      if (GetChar(_run) == '\x00') {
        NullProc();
      }
      else {
        _tokenId = TokenKind.tkString;
        if (_run > 0 || Range != SqlRange.rsString || GetChar(_run) != '\x27') {
          Range = SqlRange.rsString;
          char c;
          do {
            _run++;
            c = GetChar(_run);
          } while (!(c == '\x00' || c == '\x0A' || c == '\x0D' || c == '\x27'));

          if (GetChar(_run) == '\x27') {
            _run++;
            Range = SqlRange.rsUnknown;
          }
        }
      }
    }

    private void DoProcTable(char chr) {
      switch (chr) {
        case '\x00':
          NullProc();
          break;
        case '\x0A':
          LFProc();
          break;
        case '\x0D':
          CRProc();
          break;
        case '\x27':
          AsciiCharProc();
          break;

        case '=':
          EqualProc();
          break;
        case '>':
          GreaterProc();
          break;
        case '<':
          LowerProc();
          break;
        case '-':
          MinusProc();
          break;
        case '|':
          OrSymbolProc();
          break;
        case '+':
          PlusProc();
          break;
        case '/':
          SlashProc();
          break;
        case '&':
          AndSymbolProc();
          break;
        case '\x22':
          QuoteProc();
          break;
        case ':':
        case '@':
          VariableProc();
          break;
        case '^':
        case '%':
        case '*':
        case '!':
          SymbolAssignProc();
          break;
        case '{':
        case '}':
        case '.':
        case ',':
        case ';':
        case '?':
        case '(':
        case ')':
        case ']':
        case '~':
          SymbolProc();
          break;
        case '[':
          BracketProc();
          break;
        default:
          DoInsideProc(chr);
          break;
      }
    }

    private void DoInsideProc(char chr) {
      if ((chr >= 'A' && chr <= 'Z') || (chr >= 'a' && chr <= 'z') || (chr == '_') || (chr == '#')) {
        IdentProc();
        return;
      }

      if (chr >= '0' && chr <= '9') {
        NumberProc();
        return;
      }

      if ((chr >= '\x00' && chr <= '\x09') || (chr >= '\x0B' && chr <= '\x0C') || (chr >= '\x0E' && chr <= '\x20')) {
        SpaceProc();
        return;
      }

      UnknownProc();
    }

    private void SpaceProc() {
      _tokenId = TokenKind.tkSpace;
      char c;
      do {
        _run++;
        c = GetChar(_run);
      } while (!(c > '\x20' || c == '\x00' || c == '\x0A' || c == '\x0D'));
    }

    private void UnknownProc() {
      _run++;
      _tokenId = TokenKind.tkUnknown;
    }

    private void NumberProc() {
      _tokenId = TokenKind.tkNumber;
      if (GetChar(_run) == '0' && (GetChar(_run + 1) == 'X' || GetChar(_run + 1) == 'x')) {
        _run += 2;
        while (HexDigits.IndexOf(GetChar(_run)) != -1) _run++;
        return;
      }

      _run++;
      _tokenId = TokenKind.tkNumber;
      while (NumberStr.IndexOf(GetChar(_run)) != -1) {
        if (GetChar(_run) == '.' && GetChar(_run + 1) == '.') break;
        _run++;
      }
    }

    private void QuoteProc() {
      _tokenId = TokenKind.tkIdentifier;
      _run++;
      while (!(GetChar(_run) == '\x00' || GetChar(_run) == '\x0A' || GetChar(_run) == '\x0D')) {
        if (GetChar(_run) == '\x22') {
          _run++;
          break;
        }

        _run++;
      }
    }

    private void BracketProc() {
      _tokenId = TokenKind.tkIdentifier;
      _run++;
      while (!(GetChar(_run) == '\x00' || GetChar(_run) == '\x0A' || GetChar(_run) == '\x0D')) {
        if (GetChar(_run) == ']') {
          _run++;
          break;
        }

        _run++;
      }
    }

    private void SymbolProc() {
      _run++;
      _tokenId = TokenKind.tkSymbol;
    }

    private void SymbolAssignProc() {
      _tokenId = TokenKind.tkSymbol;
      _run++;
      if (GetChar(_run) == '=') _run++;
    }

    private void KeyHash(int pos) {
      _stringLen = 0;
      while (Array.BinarySearch(_identifiersArray, GetChar(pos)) >= 0) {
        _stringLen++;
        pos++;
      }

      return;
    }

    private TokenKind IdentKind() {
      KeyHash(_run);
      return _tokens[_line.Substring(_tokenPos, _run + _stringLen - _tokenPos)];
    }

    private void IdentProc() {
      _tokenId = IdentKind();
      _run += _stringLen;
      if (_tokenId == TokenKind.tkComment) {
        while (!(GetChar(_run) == '\x00' || GetChar(_run) == '\x0A' || GetChar(_run) == '\x0D')) {
          _run++;
        }
      }
      else {
        while (IdentifierStr.IndexOf(GetChar(_run)) != -1) _run++;
      }
    }

    private void VariableProc() {
      if (GetChar(_run) == '@' && GetChar(_run + 1) == '@') {
        _run += 2;
        IdentProc();
      }
      else {
        _tokenId = TokenKind.tkVariable;
        int i = _run;
        do {
          i++;
        } while (!(IdentifierStr.IndexOf(GetChar(i)) == -1));

        _run = i;
      }
    }

    private void AndSymbolProc() {
      _tokenId = TokenKind.tkSymbol;
      _run++;
      if (GetChar(_run) == '=' || GetChar(_run) == '&') _run++;
    }

    private void SlashProc() {
      _run++;
      switch (GetChar(_run)) {
        case '*': {
          Range = SqlRange.rsComment;
          _tokenId = TokenKind.tkComment;
          do {
            _run++;
            if (GetChar(_run) == '*' && GetChar(_run + 1) == '/') {
              Range = SqlRange.rsUnknown;
              _run += 2;
              break;
            }
          } while (!(GetChar(_run) == '\x00' || GetChar(_run) == '\x0D' || GetChar(_run) == '\x0A'));
        }
          break;
        case '=':
          _run++;
          _tokenId = TokenKind.tkSymbol;
          break;
        default:
          _tokenId = TokenKind.tkSymbol;
          break;
      }
    }

    private void PlusProc() {
      _tokenId = TokenKind.tkSymbol;
      _run++;
      if (GetChar(_run) == '=' || GetChar(_run) == '=') _run++;
    }

    private void OrSymbolProc() {
      _tokenId = TokenKind.tkSymbol;
      _run++;
      if (GetChar(_run) == '=' || GetChar(_run) == '|') _run++;
    }

    private void MinusProc() {
      _run++;
      if (GetChar(_run) == '-') {
        _tokenId = TokenKind.tkComment;
        char c;
        do {
          _run++;
          c = GetChar(_run);
        } while (!(c == '\x00' || c == '\x0A' || c == '\x0D'));
      }
      else {
        _tokenId = TokenKind.tkSymbol;
      }
    }

    private void LowerProc() {
      _tokenId = TokenKind.tkSymbol;
      _run++;
      switch (GetChar(_run)) {
        case '=':
          _run++;
          break;
        case '<':
          _run++;
          if (GetChar(_run) == '=') _run++;
          break;
      }
    }

    private void GreaterProc() {
      _tokenId = TokenKind.tkSymbol;
      _run++;
      if (GetChar(_run) == '=' || GetChar(_run) == '>') _run++;
    }

    private void EqualProc() {
      _tokenId = TokenKind.tkSymbol;
      _run++;
      if (GetChar(_run) == '=' || GetChar(_run) == '>') _run++;
    }

    private void Next() {
      _tokenPos = _run;
      switch (Range) {
        case SqlRange.rsComment:
          AnsiCProc();
          break;
        case SqlRange.rsString:
          AsciiCharProc();
          break;
        default:
          DoProcTable(GetChar(_run));
          break;
      }

      _token = _line.Substring(_tokenPos, _run - _tokenPos);
    }
  }
}