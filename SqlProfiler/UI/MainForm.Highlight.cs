using sergiye.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const string SlowThresholdSetting = "SlowMs";

    /// <summary>Events that took at least so many milliseconds are highlighted, 0 switches it off.</summary>
    private int _slowThresholdMs;

    private void InitHighlight() {
      _slowThresholdMs = _settings.GetValue(SlowThresholdSetting, 1000);

      var parent = new ToolStripMenuItem("Highlight slow events");
      foreach (var (text, ms) in new[] {("Off", 0), ("100 ms and more", 100), ("500 ms and more", 500), ("1 s and more", 1000), ("5 s and more", 5000)}) {
        var threshold = ms;
        var item = new ToolStripRadioButtonMenuItem(text, null, (sender, _) => {
          if (sender is ToolStripRadioButtonMenuItem clicked) clicked.Checked = true;
          _slowThresholdMs = threshold;
          _settings.SetValue(SlowThresholdSetting, threshold);
          lvEvents.Invalidate();
        }) {Checked = threshold == _slowThresholdMs};
        parent.DropDownItems.Add(item);
      }

      viewToolStripMenuItem.DropDownItems.Add(parent);
    }

    /// <summary>The background of the row: errors and blocked processes are marked stronger than slow events, other rows keep the default one.</summary>
    private Color? GetRowBackColor(EventRow row) {
      if (row.IsMarker) return null;
      var eventClass = row.Event.EventClass;
      if (eventClass == ProfilerEvents.ErrorsAndWarnings.UserErrorMessage
          || eventClass == ProfilerEvents.ErrorsAndWarnings.Exception
          || eventClass == ProfilerEvents.ErrorsAndWarnings.Blockedprocessreport)
        return ThemeDraw.Mix(Theme.Current.BackgroundColor, Theme.Current.WarnColor, 0.4);

      if (_slowThresholdMs > 0 && row.Event.Duration / 1000 >= _slowThresholdMs)
        return ThemeDraw.Mix(Theme.Current.BackgroundColor, Theme.Current.WarnColor, 0.2);

      return null;
    }
  }
}
