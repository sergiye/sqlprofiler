using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlProfiler {
  internal enum QuickField {
    Application,
    Login,
    Database,
    Host,
    Spid
  }

  /// <summary>
  /// Filter of the captured events built from the context menu: every field has a list of the values to show (an event must have one of
  /// them) and a list of the values to hide. The values are compared as a whole, without the case.
  /// </summary>
  internal sealed class QuickFilter {
    private readonly Dictionary<QuickField, HashSet<string>> _include = new();
    private readonly Dictionary<QuickField, HashSet<string>> _exclude = new();

    public bool IsEmpty => _include.Values.All(s => s.Count == 0) && _exclude.Values.All(s => s.Count == 0);

    public void Include(QuickField field, string value) {
      Set(_exclude, field).Remove(value);
      Set(_include, field).Add(value);
    }

    public void Exclude(QuickField field, string value) {
      Set(_include, field).Remove(value);
      Set(_exclude, field).Add(value);
    }

    public void Clear() {
      _include.Clear();
      _exclude.Clear();
    }

    private static HashSet<string> Set(Dictionary<QuickField, HashSet<string>> map, QuickField field) {
      if (!map.TryGetValue(field, out var set))
        map[field] = set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      return set;
    }

    private static string GetValue(ProfilerEvent evt, QuickField field) {
      switch (field) {
        case QuickField.Application: return evt.ApplicationName;
        case QuickField.Login: return evt.LoginName;
        case QuickField.Database: return evt.DatabaseName;
        case QuickField.Host: return evt.HostName;
        default: return evt.SPID.ToString();
      }
    }

    public bool Matches(ProfilerEvent evt) {
      foreach (var pair in _include) {
        if (pair.Value.Count > 0 && !pair.Value.Contains(GetValue(evt, pair.Key) ?? "")) return false;
      }

      foreach (var pair in _exclude) {
        if (pair.Value.Count > 0 && pair.Value.Contains(GetValue(evt, pair.Key) ?? "")) return false;
      }

      return true;
    }

    /// <summary>A short text for the status bar, for example "Application: A, B; not Login: bob".</summary>
    public string Describe() {
      var parts = new List<string>();
      foreach (QuickField field in Enum.GetValues(typeof(QuickField))) {
        var name = field == QuickField.Spid ? "SPID" : field.ToString();
        if (_include.TryGetValue(field, out var include) && include.Count > 0)
          parts.Add($"{name}: {string.Join(", ", include)}");
        if (_exclude.TryGetValue(field, out var exclude) && exclude.Count > 0)
          parts.Add($"not {name}: {string.Join(", ", exclude)}");
      }

      return string.Join("; ", parts);
    }
  }
}
