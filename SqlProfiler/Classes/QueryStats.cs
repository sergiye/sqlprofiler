using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlProfiler {
  /// <summary>The events of one query (the same normalized text) with their totals.</summary>
  internal sealed class QueryGroup {
    public QueryGroup(string text) {
      Text = text;
    }

    /// <summary>The normalized text of the query.</summary>
    public string Text { get; }

    public List<EventRow> Rows { get; } = new();

    public int Count => Rows.Count;
    public long TotalDurationMs { get; private set; }
    public long MaxDurationMs { get; private set; }
    public long TotalCpu { get; private set; }
    public long TotalReads { get; private set; }
    public long TotalWrites { get; private set; }

    public long AverageDurationMs => Count == 0 ? 0 : TotalDurationMs / Count;
    public long AverageCpu => Count == 0 ? 0 : TotalCpu / Count;

    public void Add(EventRow row) {
      Rows.Add(row);
      var duration = row.Event.Duration / 1000;
      TotalDurationMs += duration;
      MaxDurationMs = Math.Max(MaxDurationMs, duration);
      TotalCpu += row.Event.CPU;
      TotalReads += row.Event.Reads;
      TotalWrites += row.Event.Writes;
    }
  }

  internal static class QueryStats {
    /// <summary>Groups the events by the normalized text of the query; the group with the largest total duration is first.</summary>
    public static List<QueryGroup> Aggregate(IEnumerable<EventRow> rows) {
      var groups = new Dictionary<string, QueryGroup>(StringComparer.OrdinalIgnoreCase);
      foreach (var row in rows) {
        if (row.IsMarker || string.IsNullOrWhiteSpace(row.Event.TextData)) continue;
        var key = QueryNormalizer.Normalize(row.Event.TextData);
        if (key.Length == 0) continue;
        if (!groups.TryGetValue(key, out var group))
          groups[key] = group = new QueryGroup(key);
        group.Add(row);
      }

      return groups.Values.OrderByDescending(g => g.TotalDurationMs).ThenByDescending(g => g.Count).ToList();
    }
  }
}
