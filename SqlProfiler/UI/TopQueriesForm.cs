using sergiye.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SortOrder = System.Windows.Forms.SortOrder;

namespace SqlProfiler {
  /// <summary>The captured events grouped by query: how many times each query was run and how much time, CPU and reads it took.</summary>
  internal sealed class TopQueriesForm : Form {
    private readonly Func<List<QueryGroup>> _load;
    private readonly ListViewExtended _list;
    private List<QueryGroup> _groups = new();
    private int _sortColumn = 2;
    private SortOrder _sortOrder = SortOrder.Descending;

    /// <summary>The group the user chose to show in the list of events, or null.</summary>
    public QueryGroup Chosen { get; private set; }

    public TopQueriesForm(Func<List<QueryGroup>> load) {
      _load = load;
      Text = "Top queries";
      StartPosition = FormStartPosition.CenterParent;
      MinimumSize = new Size(700, 350);
      ClientSize = new Size(1100, 600);
      Font = SystemFonts.MessageBoxFont;
      ShowInTaskbar = false;

      _list = new ListViewExtended {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        MultiSelect = false,
        HideSelection = false,
        BorderStyle = BorderStyle.None
      };
      AddColumn("Query (values replaced with ?)", 520, HorizontalAlignment.Left);
      AddColumn("Count", 70, HorizontalAlignment.Right);
      AddColumn("Total ms", 100, HorizontalAlignment.Right);
      AddColumn("Avg ms", 80, HorizontalAlignment.Right);
      AddColumn("Max ms", 80, HorizontalAlignment.Right);
      AddColumn("Total CPU", 90, HorizontalAlignment.Right);
      AddColumn("Avg CPU", 80, HorizontalAlignment.Right);
      AddColumn("Total reads", 100, HorizontalAlignment.Right);
      AddColumn("Total writes", 100, HorizontalAlignment.Right);
      _list.ColumnClick += (_, e) => Sort(e.Column);
      _list.DoubleClick += (_, _) => ChooseSelected();
      _list.KeyDown += (_, e) => {
        if (e.KeyCode == Keys.Enter) ChooseSelected();
        else if (e.Control && e.KeyCode == Keys.C) CopySelected();
      };

      var show = new Button {Text = "Select in the list", AutoSize = true, Padding = new Padding(8, 2, 8, 2)};
      show.Click += (_, _) => ChooseSelected();
      var copy = new Button {Text = "Copy query", AutoSize = true, Padding = new Padding(8, 2, 8, 2)};
      copy.Click += (_, _) => CopySelected();
      var refresh = new Button {Text = "Refresh", AutoSize = true, Padding = new Padding(8, 2, 8, 2)};
      refresh.Click += (_, _) => Reload();
      var close = new Button {Text = "Close", AutoSize = true, Padding = new Padding(8, 2, 8, 2), DialogResult = DialogResult.Cancel};

      var buttons = new FlowLayoutPanel {Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6)};
      buttons.Controls.AddRange(new Control[] {close, refresh, copy, show});

      Controls.Add(_list);
      Controls.Add(buttons);
      CancelButton = close;
      ThemeSupport.Attach(this);
      Shown += (_, _) => Reload();
    }

    private void AddColumn(string caption, int width, HorizontalAlignment alignment) {
      _list.Columns.Add(new ColumnHeader {Text = caption, Width = (int) (width * DeviceDpi / 96f), TextAlign = alignment});
    }

    private void Reload() {
      _groups = _load();
      Sort(_sortColumn, true);
      Text = $"Top queries: {_groups.Count:#,0} different, {_groups.Sum(g => g.Count):#,0} events";
    }

    private void Sort(int column, bool keepOrder = false) {
      if (!keepOrder) {
        _sortOrder = column == _sortColumn && _sortOrder == SortOrder.Descending ? SortOrder.Ascending : SortOrder.Descending;
        if (column == 0 && column != _sortColumn) _sortOrder = SortOrder.Ascending;
        _sortColumn = column;
      }

      Func<QueryGroup, IComparable> key = _sortColumn switch {
        0 => g => g.Text.ToLowerInvariant(),
        1 => g => g.Count,
        2 => g => g.TotalDurationMs,
        3 => g => g.AverageDurationMs,
        4 => g => g.MaxDurationMs,
        5 => g => g.TotalCpu,
        6 => g => g.AverageCpu,
        7 => g => g.TotalReads,
        _ => g => g.TotalWrites
      };
      _groups = _sortOrder == SortOrder.Ascending ? _groups.OrderBy(key).ToList() : _groups.OrderByDescending(key).ToList();
      _list.SortColumn = _sortColumn;
      _list.SortOrder = _sortOrder;

      _list.BeginUpdate();
      try {
        _list.Items.Clear();
        foreach (var group in _groups) {
          var item = new ListViewItem(group.Text.Length > 400 ? group.Text.Substring(0, 400) + "..." : group.Text) {Tag = group};
          item.SubItems.AddRange(new[] {
            group.Count.ToString("#,0"), group.TotalDurationMs.ToString("#,0"), group.AverageDurationMs.ToString("#,0"),
            group.MaxDurationMs.ToString("#,0"), group.TotalCpu.ToString("#,0"), group.AverageCpu.ToString("#,0"),
            group.TotalReads.ToString("#,0"), group.TotalWrites.ToString("#,0")
          });
          _list.Items.Add(item);
        }
      }
      finally {
        _list.EndUpdate();
      }
    }

    private QueryGroup Selected => _list.SelectedItems.Count > 0 ? _list.SelectedItems[0].Tag as QueryGroup : null;

    private void ChooseSelected() {
      Chosen = Selected;
      if (Chosen is null) return;
      DialogResult = DialogResult.OK;
    }

    private void CopySelected() {
      var group = Selected;
      if (group is not null) Clipboard.SetText(group.Text);
    }
  }
}
