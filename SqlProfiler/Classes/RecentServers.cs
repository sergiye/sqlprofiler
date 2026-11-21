using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlProfiler {
  /// <summary>The servers used last, the latest one first. They are kept as one string, so the list follows the portable/registry choice of the settings.</summary>
  internal static class RecentServers {
    private const char Separator = '|';
    public const int MaxCount = 10;

    public static List<string> Parse(string stored) {
      return (stored ?? "").Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim())
        .Where(s => s.Length > 0)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Take(MaxCount)
        .ToList();
    }

    /// <summary>Puts the server first; returns the new stored value.</summary>
    public static string Add(string stored, string server) {
      server = (server ?? "").Replace(Separator, ' ').Trim();
      var list = Parse(stored);
      if (server.Length == 0) return string.Join(Separator.ToString(), list);
      list.RemoveAll(s => string.Equals(s, server, StringComparison.OrdinalIgnoreCase));
      list.Insert(0, server);
      return string.Join(Separator.ToString(), list.Take(MaxCount));
    }
  }
}
