using System;
using System.Linq;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private ToolStripMenuItem _miCopyText;
    private ToolStripMenuItem _miCopyOneLine;
    private ToolStripMenuItem _miSearchText;
    private ToolStripMenuItem _miOnlySpid;
    private ToolStripMenuItem _miFilterBy;

    private void InitRowMenu() {
      _miCopyText = new ToolStripMenuItem("Copy text", null, (_, _) => CopySelectedQueries(false)) {ShortcutKeyDisplayString = "Ctrl+C"};
      _miCopyOneLine = new ToolStripMenuItem("Copy as one line", null, (_, _) => CopySelectedQueries(true));
      _miSearchText = new ToolStripMenuItem("Search for this text", null, (_, _) => SearchForTextOfCurrentRow());
      _miOnlySpid = new ToolStripMenuItem("Show only this SPID", null, (_, _) => ShowOnlyCurrentSpid());

      _miFilterBy = new ToolStripMenuItem("Filter by this event");

      var items = new ToolStripItem[] {_miCopyText, _miCopyOneLine, _miSearchText, new ToolStripSeparator(), _miOnlySpid, _miFilterBy, new ToolStripSeparator()};
      for (var i = 0; i < items.Length; i++)
        contextMenu.Items.Insert(i, items[i]);

      contextMenu.Opening += (_, _) => UpdateRowMenu();
    }

    /// <summary>The row the commands of the menu work with: the focused one or the first selected one.</summary>
    private EventRow CurrentRow {
      get {
        var index = FocusedIndex;
        if (index < 0 || index >= _rows.Count || !lvEvents.SelectedIndices.Contains(index))
          index = lvEvents.SelectedIndices.Count > 0 ? lvEvents.SelectedIndices[0] : -1;
        return index >= 0 && index < _rows.Count ? _rows[index] : null;
      }
    }

    private void UpdateRowMenu() {
      var row = CurrentRow;
      var hasQuery = lvEvents.SelectedIndices.Cast<int>().Any(i => i < _rows.Count && !string.IsNullOrEmpty(_rows[i].Event.TextData));
      _miCopyText.Enabled = hasQuery;
      _miCopyOneLine.Enabled = hasQuery;
      _miSearchText.Enabled = row is not null && !string.IsNullOrEmpty(SelectedOrRowText(row));
      _miOnlySpid.Enabled = row is not null && !row.IsMarker;
      FillFilterByMenu(row);
    }

    /// <summary>"Show only"/"Hide" commands for the application, login, database and host of the row.</summary>
    private void FillFilterByMenu(EventRow row) {
      _miFilterBy.DropDownItems.Clear();
      _miFilterBy.Enabled = row is not null && !row.IsMarker;
      if (!_miFilterBy.Enabled) return;

      var evt = row.Event;
      AddFilterByItems("Application", evt.ApplicationName, QuickField.Application);
      AddFilterByItems("Login", evt.LoginName, QuickField.Login);
      AddFilterByItems("Database", evt.DatabaseName, QuickField.Database);
      AddFilterByItems("Host", evt.HostName, QuickField.Host);
      _miFilterBy.Enabled = _miFilterBy.DropDownItems.Count > 0;
    }

    private void AddFilterByItems(string caption, string value, QuickField field) {
      if (string.IsNullOrEmpty(value)) return;
      if (_miFilterBy.DropDownItems.Count > 0)
        _miFilterBy.DropDownItems.Add(new ToolStripSeparator());

      var shown = value.Length > 40 ? value.Substring(0, 40) + "..." : value;
      _miFilterBy.DropDownItems.Add(new ToolStripMenuItem($"Show only {caption}: {shown}", null,
        (_, _) => ApplyQuickFilter(f => f.Include(field, value))));
      _miFilterBy.DropDownItems.Add(new ToolStripMenuItem($"Hide {caption}: {shown}", null,
        (_, _) => ApplyQuickFilter(f => f.Exclude(field, value))));
    }

    /// <summary>The text selected in the text box, or the whole text of the row if nothing is selected there.</summary>
    private string SelectedOrRowText(EventRow row) {
      return reTextData.SelectionLength > 0 ? reTextData.SelectedText : row.Event.TextData;
    }

    private void SearchForTextOfCurrentRow() {
      var row = CurrentRow;
      if (row is null) return;
      var text = SelectedOrRowText(row).Trim();
      if (text.Length == 0) return;
      LastPattern = text;
      PerformFind(true, true);
    }

    private void ShowOnlyCurrentSpid() {
      var row = CurrentRow;
      if (row is null || row.IsMarker) return;
      var spid = row.Event.SPID.ToString();
      ApplyQuickFilter(f => f.Include(QuickField.Spid, spid));
    }
  }
}
