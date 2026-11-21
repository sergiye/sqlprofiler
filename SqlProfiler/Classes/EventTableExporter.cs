using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SqlProfiler {
  /// <summary>CSV and JSON export. Every event is written with all its columns and with raw (not formatted) values.</summary>
  internal static class EventTableExporter {
    private const string TimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    private sealed class Field {
      public Field(string name, bool isNumber, Func<EventRow, object> get) {
        Name = name;
        IsNumber = isNumber;
        Get = get;
      }

      public string Name { get; }
      public bool IsNumber { get; }
      public Func<EventRow, object> Get { get; }
    }

    private static readonly Field[] Fields = {
      new("Number", true, r => r.Number),
      new("EventClass", false, r => r.Caption),
      new("TextData", false, r => r.Event.TextData),
      new("LoginName", false, r => r.Event.LoginName),
      new("CPU", true, r => r.Event.CPU),
      new("Reads", true, r => r.Event.Reads),
      new("Writes", true, r => r.Event.Writes),
      new("DurationMs", true, r => r.Event.Duration / 1000),
      new("SPID", true, r => r.Event.SPID),
      new("StartTime", false, r => FormatTime(r.Event.StartTime)),
      new("EndTime", false, r => FormatTime(r.Event.EndTime)),
      new("DatabaseName", false, r => r.Event.DatabaseName),
      new("ObjectName", false, r => r.Event.ObjectName),
      new("ApplicationName", false, r => r.Event.ApplicationName),
      new("HostName", false, r => r.Event.HostName),
    };

    private static string FormatTime(DateTime time) {
      return time.Year <= 1 ? null : time.ToString(TimeFormat, CultureInfo.InvariantCulture);
    }

    private static string ToText(object value) {
      return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
    }

    public static string BuildCsv(IEnumerable<EventRow> rows) {
      var sb = new StringBuilder();
      sb.Append(string.Join(",", Fields.Select(f => f.Name))).Append("\r\n");
      foreach (var row in rows) {
        sb.Append(string.Join(",", Fields.Select(f => CsvValue(ToText(f.Get(row)), f.IsNumber)))).Append("\r\n");
      }

      return sb.ToString();
    }

    private static string CsvValue(string text, bool isNumber) {
      if (isNumber || text.IndexOfAny(new[] {',', '"', '\r', '\n'}) < 0)
        return text;
      return "\"" + text.Replace("\"", "\"\"") + "\"";
    }

    public static string BuildJson(IEnumerable<EventRow> rows) {
      var sb = new StringBuilder("[");
      var first = true;
      foreach (var row in rows) {
        sb.Append(first ? "\r\n" : ",\r\n").Append("  {");
        first = false;
        for (var i = 0; i < Fields.Length; i++) {
          var field = Fields[i];
          sb.Append(i == 0 ? "\r\n    " : ",\r\n    ").Append('"').Append(field.Name).Append("\": ");
          var value = field.Get(row);
          if (value is null) sb.Append("null");
          else if (field.IsNumber) sb.Append(ToText(value));
          else sb.Append(JsonString(ToText(value)));
        }

        sb.Append("\r\n  }");
      }

      return sb.Append(first ? "]" : "\r\n]").ToString();
    }

    private static string JsonString(string text) {
      var sb = new StringBuilder("\"");
      foreach (var c in text) {
        switch (c) {
          case '"': sb.Append("\\\""); break;
          case '\\': sb.Append("\\\\"); break;
          case '\n': sb.Append("\\n"); break;
          case '\r': sb.Append("\\r"); break;
          case '\t': sb.Append("\\t"); break;
          default:
            if (c < 0x20) sb.Append("\\u").Append(((int) c).ToString("x4"));
            else sb.Append(c);
            break;
        }
      }

      return sb.Append('"').ToString();
    }
  }
}
