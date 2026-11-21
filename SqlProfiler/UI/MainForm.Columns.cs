using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const int MinColumnWidth = 40;

    private const string LiveSortSetting = "LiveSort";

    private ContextMenuStrip _columnsMenu;
    private bool _liveSort;

    /// <summary>View - "Keep sorted while tracing": the new events are put into the sorted list at once, not only when the trace stops.</summary>
    private void InitLiveSort() {
      _liveSort = _settings.GetValue(LiveSortSetting, 0) == 1;
      var item = new ToolStripMenuItem("Keep sorted while tracing") {
        CheckOnClick = true,
        Checked = _liveSort,
        ToolTipText = "Sort the new events at once (otherwise the list is sorted when the trace stops)"
      };
      item.CheckedChanged += (_, _) => {
        _liveSort = item.Checked;
        _settings.SetValue(LiveSortSetting, _liveSort ? 1 : 0);
        if (_liveSort) ApplySort();
      };
      viewToolStripMenuItem.DropDownItems.Add(item);
    }

    private float DpiScale => DeviceDpi / 96f;

    /// <summary>The columns that can be shown or hidden: all of them are always captured, so this can be changed at any time.</summary>
    private (string Caption, Func<bool> Get, Action<bool> Set)[] OptionalColumns {
      get {
        var c = _traceSettings.EventsColumns;
        return new (string, Func<bool>, Action<bool>)[] {
          ("Start time", () => c.StartTime, v => c.StartTime = v),
          ("End time", () => c.EndTime, v => c.EndTime = v),
          ("Database name", () => c.DatabaseName, v => c.DatabaseName = v),
          ("Object name", () => c.ObjectName, v => c.ObjectName = v),
          ("Application name", () => c.ApplicationName, v => c.ApplicationName = v),
          ("Host name", () => c.HostName, v => c.HostName = v),
        };
      }
    }

    /// <summary>A right click on the column header shows the menu of the columns.</summary>
    private void ShowColumnsMenu(System.Drawing.Point screenPoint) {
      _columnsMenu?.Dispose();
      _columnsMenu = new ContextMenuStrip();
      foreach (var column in OptionalColumns) {
        var item = new ToolStripMenuItem(column.Caption) {Checked = column.Get(), CheckOnClick = false};
        var current = column;
        item.Click += (_, _) => {
          current.Set(!current.Get());
          RebuildColumns();
        };
        _columnsMenu.Items.Add(item);
      }

      _columnsMenu.Items.Add(new ToolStripSeparator());
      _columnsMenu.Items.Add(new ToolStripMenuItem("Reset column widths", null, (_, _) => ResetColumnWidths()));
      _columnsMenu.Show(screenPoint);
    }

    /// <summary>Applies the changed set of the columns and keeps the widths and the sorting.</summary>
    private void RebuildColumns() {
      SaveColumnWidths();
      var sortCaption = lvEvents.SortColumn >= 0 && lvEvents.SortColumn < _columns.Count ? _columns[lvEvents.SortColumn].Caption : null;
      InitGridColumns();
      var sortIndex = sortCaption is null ? -1 : _columns.FindIndex(c => c.Caption == sortCaption);
      if (sortIndex < 0) lvEvents.SortOrder = SortOrder.None;
      lvEvents.SortColumn = sortIndex;
      lvEvents.Invalidate();
    }

    private void ResetColumnWidths() {
      foreach (var column in _columns)
        _settings.Remove(ColumnWidthPrefix + column.Caption);
      InitGridColumns();
      lvEvents.Invalidate();
    }

    private int MinTextColumnWidth => (int) Math.Round(MinFixedColumnWidth * DpiScale);

    /// <summary>
    /// Makes the default widths of the columns fit the list: if all the columns do not fit, the columns whose width the user has not
    /// changed are narrowed proportionally, so that the Text Data column keeps at least its minimal width and no scroll bar appears.
    /// </summary>
    private void FitColumnsToWidth(List<PerfColumn> columns) {
      var available = lvEvents.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
      if (available <= 0) return;

      var others = columns.Where((_, i) => i != GridColumns.TextDataIndex).ToList();
      var room = available - MinTextColumnWidth;
      var othersTotal = others.Sum(c => c.Width);
      if (othersTotal <= room) return;

      var flexible = others.Where(c => !c.UserWidth).ToList();
      var flexibleTotal = flexible.Sum(c => c.Width);
      var target = room - (othersTotal - flexibleTotal);
      if (flexibleTotal <= 0 || target <= 0) return;

      var factor = Math.Min(1.0, (double) target / flexibleTotal);
      foreach (var column in flexible)
        column.Width = Math.Max(MinColumnWidth, (int) (column.Width * factor));
    }
  }
}
