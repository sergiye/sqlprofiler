namespace SqlProfiler {
  /// <summary>Applies the trace filters to the already captured events.</summary>
  internal static class EventFilter {
    public static bool IsIncluded(TraceProperties.TraceFilters filters, EventRow row) {
      if (row.IsMarker) return true;
      var evt = row.Event;
      return IsIncluded(filters.ApplicationNameFilterCondition, filters.ApplicationName, evt.ApplicationName)
             && IsIncluded(filters.TextDataFilterCondition, filters.TextData, evt.TextData)
             && IsIncluded(filters.LoginNameFilterCondition, filters.LoginName, evt.LoginName)
             && IsIncluded(filters.DatabaseNameFilterCondition, filters.DatabaseName, evt.DatabaseName)
             && IsIncluded(filters.HostNameFilterCondition, filters.HostName, evt.HostName)
             && IsIncluded(filters.CpuFilterCondition, filters.CPU, evt.CPU)
             && IsIncluded(filters.ReadsFilterCondition, filters.Reads, evt.Reads)
             && IsIncluded(filters.WritesFilterCondition, filters.Writes, evt.Writes)
             // the duration is shown (and filtered) in milliseconds
             && IsIncluded(filters.DurationFilterCondition, filters.Duration, evt.Duration / 1000)
             && IsIncluded(filters.SPIDFilterCondition, filters.SPID, evt.SPID);
    }

    private static bool IsIncluded(TraceProperties.StringFilterCondition condition, string filter, string value) {
      if (string.IsNullOrEmpty(filter)) return true;
      var contains = (value ?? "").Contains(filter);
      return condition == TraceProperties.StringFilterCondition.Like ? contains : !contains;
    }

    private static bool IsIncluded(TraceProperties.IntFilterCondition condition, int? filter, long value) {
      if (filter is null) return true;
      return condition switch {
        TraceProperties.IntFilterCondition.Equal => value == filter,
        TraceProperties.IntFilterCondition.NotEqual => value != filter,
        TraceProperties.IntFilterCondition.GreaterThan => value > filter,
        TraceProperties.IntFilterCondition.LessThan => value < filter,
        _ => true
      };
    }
  }
}
