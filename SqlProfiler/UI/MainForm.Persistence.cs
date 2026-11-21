using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const string TraceSettingsKey = "TraceSettings";
    private const string ColumnWidthPrefix = "ColW_";

    /// <summary>The events, columns and filters of the previous run, or the defaults.</summary>
    private TraceProperties.TraceSettings LoadTraceSettings() {
      return TraceSettingsXml.TryDeserialize(_settings.GetValue(TraceSettingsKey, "")) ?? TraceProperties.TraceSettings.GetDefaultSettings();
    }

    private void SaveTraceSettings() {
      _settings.SetValue(TraceSettingsKey, TraceSettingsXml.Serialize(_traceSettings));
    }

    /// <summary>The width of a column is remembered by its caption; the Text Data column takes the rest of the width.</summary>
    private void RestoreColumnWidths(List<PerfColumn> columns) {
      for (var i = 0; i < columns.Count; i++) {
        if (i == GridColumns.TextDataIndex) continue;
        var width = _settings.GetValue(ColumnWidthPrefix + columns[i].Caption, columns[i].Width);
        if (width < 20 || width > 4000) continue;
        columns[i].Width = width;
        columns[i].UserWidth = _settings.Contains(ColumnWidthPrefix + columns[i].Caption);
      }
    }

    private void SaveColumnWidths() {
      for (var i = 0; i < _columns.Count && i < lvEvents.Columns.Count; i++) {
        if (i == GridColumns.TextDataIndex) continue;
        _settings.SetValue(ColumnWidthPrefix + _columns[i].Caption, lvEvents.Columns[i].Width);
      }
    }

    private void SaveLayout() {
      var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
      _settings.SetValue("Left", bounds.Left);
      _settings.SetValue("Top", bounds.Top);
      _settings.SetValue("Height", bounds.Height);
      _settings.SetValue("Width", bounds.Width);
      _settings.SetValue(SourceZoomSetting, (int) Math.Round(reTextData.ZoomFactor * 100));
      _settings.SetValue("Maximized", WindowState == FormWindowState.Maximized ? 1 : 0);
      if (splitContainer1.Height > 0)
        _settings.SetValue("SplitRatio", splitContainer1.SplitterDistance * 100 / splitContainer1.Height);
    }

    private void RestoreLayout() {
      Left = _settings.GetValue("Left", Screen.PrimaryScreen.WorkingArea.Left + Width / 2);
      Top = _settings.GetValue("Top", Screen.PrimaryScreen.WorkingArea.Top + Height / 2);
      Height = _settings.GetValue("Height", Screen.PrimaryScreen.WorkingArea.Height / 2);
      Width = _settings.GetValue("Width", Screen.PrimaryScreen.WorkingArea.Width / 2);

      var ratio = _settings.GetValue("SplitRatio", 0);
      if (ratio >= 10 && ratio <= 90)
        splitContainer1.SplitterDistance = splitContainer1.Height * ratio / 100;

      if (_settings.GetValue("Maximized", 0) == 1)
        WindowState = FormWindowState.Maximized;
    }
  }
}
