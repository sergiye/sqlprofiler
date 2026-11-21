using System.Windows.Forms;

namespace SqlProfiler {
  internal class PerfColumn {
    public string Caption;
    public int Column;
    public int Width;

    /// <summary>The width was set by the user (restored from the settings), so it is not changed to fit the list.</summary>
    public bool UserWidth;
    public string Format;
    public HorizontalAlignment Alignment = HorizontalAlignment.Left;
  }
}