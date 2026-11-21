using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SqlProfiler {
  internal static class RowSorter {
    private readonly struct Key {
      public Key(EventRow row, string text) {
        Row = row;
        Text = text;
        IsNumber = int.TryParse(text.Replace(",", ""), out var number);
        Number = number;
      }

      public EventRow Row { get; }
      public string Text { get; }
      public bool IsNumber { get; }
      public int Number { get; }
    }

    /// <summary>Sorts the rows by the shown text of a column: numbers as numbers, the rest as text. The sort is stable.</summary>
    public static void Sort(List<EventRow> rows, int column, SortOrder order, Func<EventRow, int, string> getText) {
      var keys = rows.Select(r => new Key(r, getText(r, column) ?? "")).ToList();
      var sorted = order == SortOrder.Descending
        ? keys.OrderBy(k => k, Comparer<Key>.Create((x, y) => Compare(y, x)))
        : keys.OrderBy(k => k, Comparer<Key>.Create(Compare));
      var result = sorted.Select(k => k.Row).ToList();
      rows.Clear();
      rows.AddRange(result);
    }

    private static int Compare(Key x, Key y) {
      return x.IsNumber && y.IsNumber ? x.Number.CompareTo(y.Number) : string.Compare(x.Text, y.Text, false);
    }
  }
}
