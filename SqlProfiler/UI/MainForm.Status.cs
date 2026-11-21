using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private readonly Stopwatch _traceClock = new();
    private ToolStripStatusLabel _slState;
    private ToolStripStatusLabel _slServer;
    private ToolStripStatusLabel _slElapsed;
    private string _activeServer = "";

    private void InitStatusBar() {
      _slState = new ToolStripStatusLabel {AutoSize = false, Width = 90, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ToolTipText = "State of the trace"};
      _slServer = new ToolStripStatusLabel {AutoSize = false, Width = 220, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ToolTipText = "Server of the trace"};
      _slElapsed = new ToolStripStatusLabel {AutoSize = false, Width = 90, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ToolTipText = "Time the trace has been running"};
      statusStrip1.Items.Insert(0, _slState);
      statusStrip1.Items.Insert(1, _slServer);
      statusStrip1.Items.Insert(2, _slElapsed);
      UpdateStatus();
    }

    private void UpdateStatus() {
      if (_slState is null) return;
      _slState.Text = _session.State switch {
        ProfilingStateEnum.psProfiling => "Running",
        ProfilingStateEnum.psPaused => "Paused",
        _ => "Stopped"
      };
      _slServer.Text = _session.State == ProfilingStateEnum.psStopped ? "" : _activeServer;
      _slElapsed.Text = FormatElapsed(_traceClock.Elapsed);
    }

    private static string FormatElapsed(TimeSpan elapsed) {
      return $"{(int) elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
    }
  }
}
