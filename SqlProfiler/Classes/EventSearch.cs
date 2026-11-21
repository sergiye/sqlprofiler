using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SqlProfiler {
  internal static class EventSearch {
    /// <summary>The indexes of all the rows whose text data contains the pattern.</summary>
    public static List<int> FindAll(IReadOnlyList<EventRow> rows, string pattern, bool matchCase, bool wholeWord) {
      var result = new List<int>();
      if (string.IsNullOrEmpty(pattern)) return result;
      var regex = CreateRegex(pattern, matchCase, wholeWord);
      for (var i = 0; i < rows.Count; i++)
        if (regex.IsMatch(rows[i].Event.TextData)) result.Add(i);
      return result;
    }

    private static Regex CreateRegex(string pattern, bool matchCase, bool wholeWord) {
      var escaped = Regex.Escape(pattern);
      return new Regex(wholeWord ? "\\b" + escaped + "\\b" : escaped, matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
    }

    /// <summary>Returns the index of the next row whose text data contains the pattern, or -1. The pattern is a plain text.</summary>
    public static int Find(IReadOnlyList<EventRow> rows, string pattern, bool matchCase, bool wholeWord,
      bool forwards, bool wrapAround, int currentIndex) {
      if (string.IsNullOrEmpty(pattern) || rows.Count == 0) return -1;

      var regex = CreateRegex(pattern, matchCase, wholeWord);

      if (forwards) {
        for (var i = currentIndex + 1; i < rows.Count; i++)
          if (regex.IsMatch(rows[i].Event.TextData)) return i;
        if (wrapAround)
          for (var i = 0; i <= currentIndex && i < rows.Count; i++)
            if (regex.IsMatch(rows[i].Event.TextData)) return i;
      }
      else {
        for (var i = currentIndex - 1; i >= 0; i--)
          if (regex.IsMatch(rows[i].Event.TextData)) return i;
        if (wrapAround)
          for (var i = rows.Count - 1; i >= currentIndex && i >= 0; i--)
            if (regex.IsMatch(rows[i].Event.TextData)) return i;
      }

      return -1;
    }
  }
}
