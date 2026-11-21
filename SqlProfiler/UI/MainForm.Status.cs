using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private readonly Stopwatch _traceClock = new();
    private ToolStripStatusLabel _slState;
    private ToolStripStatusLabel _slServer;
    private ToolStripStatusLabel _slElapsed;
    private ToolStripStatusLabel _slEvents;
    private ToolStripStatusLabel _slFilter;
    private int _trimmedCount;
    private string _activeServer = "";

    private void InitStatusBar() {
      _slState = new ToolStripStatusLabel {AutoSize = false, Width = 90, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ToolTipText = "State of the trace"};
      _slServer = new ToolStripStatusLabel {AutoSize = false, Width = 220, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ToolTipText = "Server of the trace"};
      _slElapsed = new ToolStripStatusLabel {AutoSize = false, Width = 90, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ToolTipText = "Time the trace has been running"};
      statusStrip1.Items.Insert(0, _slState);
      statusStrip1.Items.Insert(1, _slServer);
      statusStrip1.Items.Insert(2, _slElapsed);
      _slEvents = new ToolStripStatusLabel {AutoSize = false, Width = 420, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ToolTipText = "Events in the list"};
      statusStrip1.Items.Insert(3, _slEvents);
      _slFilter = new ToolStripStatusLabel {IsLink = true, LinkBehavior = LinkBehavior.HoverUnderline, Visible = false, ToolTipText = "Click to show all the events"};
      _slFilter.Click += (_, _) => ClearFilterEvents();
      statusStrip1.Items.Insert(4, _slFilter);
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
      UpdateEventsStatus();
    }

    /// <summary>Shows how many events are in the list and, if the limit has already pushed some of them out, how many were removed.</summary>
    private void UpdateEventsStatus() {
      if (_slEvents is null) return;
      var limit = Math.Max(_traceSettings?.Filters.MaximumEventCount ?? 0, 0);
      _slEvents.Text = (_trimmedCount > 0
        ? $"{_rows.Count:#,0} shown, {_trimmedCount:#,0} oldest removed (limit {limit:#,0})"
        : $"{_rows.Count:#,0} events") + (_filtering ? ", filtered" : "");
      var description = _quickFilter.IsEmpty ? "" : _quickFilter.Describe();
      _slFilter.Visible = _filtering && description.Length > 0;
      _slFilter.Text = description.Length > 0 ? "Filter: " + description + " (click to clear)" : "";
      _slEvents.ToolTipText = _trimmedCount > 0
        ? "The list keeps only the latest events. Change the limit in 'Run with filters' - Maximum events count."
        : "Events in the list";
    }

    private static string FormatElapsed(TimeSpan elapsed) {
      return $"{(int) elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
    }
  }
}
