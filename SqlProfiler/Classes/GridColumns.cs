using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SqlProfiler {
  internal static class GridColumns {
    /// <summary>The column that takes all the free width of the list.</summary>
    public const int TextDataIndex = 1;

    private const string TimeFormat = "yyyy-MM-dd HH:mm:ss.ffff";

    public static List<PerfColumn> Build(TraceProperties.TraceEventsColumns options, float scale = 1f) {
      var columns = new List<PerfColumn> {
        new() {Caption = "Event Class", Column = ProfilerEventColumns.EventClass, Width = 122},
        new() {Caption = "Text Data", Column = ProfilerEventColumns.TextData, Width = 255},
        new() {Caption = "Login Name", Column = ProfilerEventColumns.LoginName, Width = 79},
        new() {Caption = "CPU", Column = ProfilerEventColumns.CPU, Width = 82, Alignment = HorizontalAlignment.Right, Format = "#,0"},
        new() {Caption = "Reads", Column = ProfilerEventColumns.Reads, Width = 78, Alignment = HorizontalAlignment.Right, Format = "#,0"},
        new() {Caption = "Writes", Column = ProfilerEventColumns.Writes, Width = 78, Alignment = HorizontalAlignment.Right, Format = "#,0"},
        new() {Caption = "Duration, ms", Column = ProfilerEventColumns.Duration, Width = 82, Alignment = HorizontalAlignment.Right, Format = "#,0"},
        new() {Caption = "SPID", Column = ProfilerEventColumns.SPID, Width = 50, Alignment = HorizontalAlignment.Right},
      };

      if (options.StartTime)
        columns.Add(new PerfColumn {Caption = "Start time", Column = ProfilerEventColumns.StartTime, Width = 140, Format = TimeFormat});
      if (options.EndTime)
        columns.Add(new PerfColumn {Caption = "End time", Column = ProfilerEventColumns.EndTime, Width = 140, Format = TimeFormat});
      if (options.DatabaseName)
        columns.Add(new PerfColumn {Caption = "DatabaseName", Column = ProfilerEventColumns.DatabaseName, Width = 70});
      if (options.ObjectName)
        columns.Add(new PerfColumn {Caption = "Object name", Column = ProfilerEventColumns.ObjectName, Width = 70});
      if (options.ApplicationName)
        columns.Add(new PerfColumn {Caption = "Application name", Column = ProfilerEventColumns.ApplicationName, Width = 70});
      if (options.HostName)
        columns.Add(new PerfColumn {Caption = "Host name", Column = ProfilerEventColumns.HostName, Width = 70});

      columns.Add(new PerfColumn {Caption = "#", Column = -1, Width = 53, Alignment = HorizontalAlignment.Right});
      foreach (var column in columns)
        column.Width = (int) Math.Round(column.Width * scale);
      return columns;
    }
  }
}
