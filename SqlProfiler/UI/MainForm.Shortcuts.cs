using System;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private void InitShortcuts() {
      tbClear.ToolTipText = "Clear trace (Ctrl+Shift+Del)";
      tbStart.ToolTipText = "Start or resume the trace (F5, Alt+S)";
      tbPause.ToolTipText = "Pause the trace (Alt+P)";
      tbStop.ToolTipText = "Stop the trace (Shift+F5, Alt+T)";
      tbFilterEvents.ToolTipText = "Filter the captured events (switch off to show all of them)";

      mnRun.ShortcutKeyDisplayString = "F5 / Alt+S";
      stopTraceToolStripMenuItem.ShortcutKeyDisplayString = "Shift+F5 / Alt+T";
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      switch (keyData) {
        case Keys.F5:
          if (tbRun.Enabled) TbStart_Click(this, EventArgs.Empty);
          return true;
        case Keys.Shift | Keys.F5:
          if (tbStop.Enabled) StopProfiling();
          return true;
        case Keys.Shift | Keys.F3:
          PerformFind(false, true);
          return true;
        case Keys.Control | Keys.C:
        case Keys.Control | Keys.Insert:
          // in the list Ctrl+C copies the text of the queries; in the text box it copies the selected text as usual
          if (!lvEvents.Focused) return base.ProcessCmdKey(ref msg, keyData);
          CopySelectedQueries(false);
          return true;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
    }
  }
}
