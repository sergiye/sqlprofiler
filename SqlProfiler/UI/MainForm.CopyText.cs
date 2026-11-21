using System;
using System.Linq;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const string BatchSeparator = "\r\ngo\r\n";

    private void InitCopyText() {
      var copyItem = new ToolStripMenuItem("Copy query text", null, (_, _) => CopySelectedQueries(false)) {ShortcutKeyDisplayString = "Ctrl+C"};
      editToolStripMenuItem.DropDownItems.Insert(0, copyItem);
      editToolStripMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
    }

    /// <summary>The text of the selected queries; several queries are separated with "go" so that the result can be run as it is.</summary>
    private string GetSelectedQueryText(bool oneLine) {
      var texts = lvEvents.SelectedIndices.Cast<int>()
        .Where(i => i < _rows.Count)
        .Select(i => _rows[i].Event.TextData)
        .Where(text => !string.IsNullOrEmpty(text))
        .Select(text => oneLine ? ToOneLine(text) : text)
        .ToList();
      return string.Join(oneLine ? Environment.NewLine : BatchSeparator, texts);
    }

    /// <summary>Joins the lines of a query with single spaces.</summary>
    internal static string ToOneLine(string text) {
      var lines = text.Split(new[] {"\r\n", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries)
        .Select(line => line.Trim())
        .Where(line => line.Length > 0);
      return string.Join(" ", lines);
    }

    private void CopySelectedQueries(bool oneLine) {
      var text = GetSelectedQueryText(oneLine);
      if (text.Length == 0) return;
      Clipboard.SetText(text);
    }
  }
}
