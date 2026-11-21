using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;

namespace SqlProfiler {
  class RTFBuilder {
    private readonly StringBuilder _sb = new StringBuilder();
    private readonly List<Color> _colortable = new List<Color>();
    private readonly StringCollection _fonttable = new StringCollection();
    private Color _forecolor;

    public Color ForeColor {
      set {
        if (!_colortable.Contains(value)) {
          _colortable.Add(value);
        }

        if (value != _forecolor) {
          _sb.Append(String.Format("\\cf{0} ", _colortable.IndexOf(value) + 1));
        }

        _forecolor = value;
      }
    }


    private Color _backcolor;

    public Color BackColor {
      set {
        if (!_colortable.Contains(value)) {
          _colortable.Add(value);
        }

        if (value != _backcolor) {
          _sb.Append(String.Format("\\highlight{0} ", _colortable.IndexOf(value) + 1));
        }

        _backcolor = value;
      }
    }


    public RTFBuilder() {
      ForeColor = Color.FromKnownColor(KnownColor.WindowText);
      BackColor = Color.FromKnownColor(KnownColor.Window);
      _defaultFontSize = 20F;
    }

    public void AppendLine() {
      _sb.AppendLine("\\line");
    }

    public void Append(string value) {
      if (!string.IsNullOrEmpty(value)) {
        value = CheckChar(value);
        if (value.IndexOf(Environment.NewLine) >= 0) {
          string[] lines = value.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
          foreach (string line in lines) {
            _sb.Append(line);
            _sb.Append("\\line ");
          }
        }
        else {
          _sb.Append(value);
        }
      }
    }

    private static readonly char[] Slashable = new[] {'{', '}', '\\'};
    private readonly float _defaultFontSize;

    private static string CheckChar(string value) {
      if (!string.IsNullOrEmpty(value)) {
        if (value.IndexOfAny(Slashable) >= 0) {
          value = value.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}");
        }

        bool replaceuni = false;
        for (int i = 0; i < value.Length; i++) {
          if (value[i] > 255) {
            replaceuni = true;
            break;
          }
        }

        if (replaceuni) {
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < value.Length; i++) {
            if (value[i] <= 255) {
              sb.Append(value[i]);
            }
            else {
              sb.Append("\\u");
              sb.Append((short) value[i]);
              sb.Append("?");
            }
          }

          value = sb.ToString();
        }
      }


      return value;
    }

    public new string ToString() {
      StringBuilder result = new StringBuilder();
      result.Append("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang3081");
      result.Append("{\\fonttbl");
      for (int i = 0; i < _fonttable.Count; i++) {
        try {
          result.Append(string.Format(_fonttable[i], i));
        }
        catch (Exception ex) {
          Console.WriteLine(ex.Message);
        }
      }

      result.AppendLine("}");
      result.Append("{\\colortbl ;");
      foreach (Color item in _colortable) {
        result.AppendFormat("\\red{0}\\green{1}\\blue{2};", item.R, item.G, item.B);
      }

      result.AppendLine("}");
      result.Append("\\viewkind4\\uc1\\pard\\plain\\f0");
      result.AppendFormat("\\fs{0} ", _defaultFontSize);
      result.AppendLine();
      result.Append(_sb.ToString());
      result.Append("}");
      return result.ToString();
    }
  }
}