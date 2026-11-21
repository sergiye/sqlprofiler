namespace SqlProfiler {
  /// <summary>An event shown in the list. The list view items are created from it on demand.</summary>
  internal sealed class EventRow {
    public EventRow(ProfilerEvent evt, int number, string caption, bool isMarker) {
      Event = evt;
      Number = number;
      Caption = caption;
      IsMarker = isMarker;
    }

    public ProfilerEvent Event { get; }

    /// <summary>Sequence number of the event within the trace.</summary>
    public int Number { get; }

    public string Caption { get; }

    /// <summary>"Trace started/paused/stopped" row: it is not a real trace event, so it is never filtered out.</summary>
    public bool IsMarker { get; }
  }
}
