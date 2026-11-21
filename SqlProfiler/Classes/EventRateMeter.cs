using System;
using System.Collections.Generic;

namespace SqlProfiler {
  /// <summary>Calculates events per second: for the last second and for the last minute.</summary>
  internal sealed class EventRateMeter {
    private const int HistoryLength = 60;

    private sealed class Sample {
      public Sample(DateTime time, int count) {
        Time = time;
        Count = count;
      }

      public DateTime Time { get; }
      public int Count { get; }
    }

    private readonly Queue<Sample> _history = new();
    private Sample _first;
    private Sample _previous;

    public void Reset() {
      _history.Clear();
      _first = null;
      _previous = null;
    }

    /// <summary>Returns the text for the status bar, or null if less than a second has passed since the previous update.</summary>
    public string Update(int totalCount) {
      var now = DateTime.Now;
      if (_previous is not null && (now - _previous.Time).TotalSeconds < 1) return null;

      var current = new Sample(now, totalCount);
      if (_history.Count >= HistoryLength)
        _first = _history.Dequeue();
      _first ??= current;
      _previous ??= current;

      var lastSeconds = (now - _previous.Time).TotalSeconds;
      var allSeconds = (now - _first.Time).TotalSeconds;
      var text = string.Format("{0} / {1} EPS(last/avg for {2} second(s))",
        Math.Abs(lastSeconds) > 0.001 ? ((current.Count - _previous.Count) / lastSeconds).ToString("#,0.00") : "",
        Math.Abs(allSeconds) > 0.001 ? ((current.Count - _first.Count) / allSeconds).ToString("#,0.00") : "",
        allSeconds.ToString("0"));

      _history.Enqueue(current);
      _previous = current;
      return text;
    }
  }
}
