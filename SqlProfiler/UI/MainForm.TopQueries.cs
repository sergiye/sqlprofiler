using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private void InitTopQueries() {
      var item = new ToolStripMenuItem("Top queries...", null, (_, _) => OpenTopQueries()) {
        ShortcutKeys = Keys.Control | Keys.Q,
        ToolTipText = "The captured events grouped by query"
      };
      viewToolStripMenuItem.DropDownItems.Add(item);
    }

    private void OpenTopQueries() {
      if (!_rows.Any(r => !r.IsMarker)) {
        MessageBox.Show("There are no events yet.", "Top queries", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      using var form = new TopQueriesForm(() => QueryStats.Aggregate(_rows));
      if (form.ShowDialog(this) == DialogResult.OK && form.Chosen is not null)
        SelectRowsOf(form.Chosen);
    }

    /// <summary>Selects the events of the query in the list and shows the first of them.</summary>
    private void SelectRowsOf(QueryGroup group) {
      // the list must not jump to the newest event while the events of the query are selected
      if (_session.State == ProfilingStateEnum.psProfiling && autoScrollToolStripMenuItem.Checked)
        autoScrollToolStripMenuItem.Checked = false;

      var wanted = new HashSet<EventRow>(group.Rows);
      var first = -1;
      lvEvents.BeginUpdate();
      _dontUpdateSource = true;
      try {
        lvEvents.SelectedIndices.Clear();
        for (var i = 0; i < _rows.Count; i++) {
          if (!wanted.Contains(_rows[i])) continue;
          lvEvents.SelectedIndices.Add(i);
          if (first < 0) first = i;
        }
      }
      finally {
        _dontUpdateSource = false;
        lvEvents.EndUpdate();
      }

      _prevSelectedIndices = null;
      UpdateSourceBox();
      if (first >= 0) {
        lvEvents.Items[first].Focused = true;
        lvEvents.EnsureVisible(first);
      }
    }
  }
}
