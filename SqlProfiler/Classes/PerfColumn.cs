using System.Windows.Forms;

namespace SqlProfiler {
  internal class PerfColumn {
    public string Caption;
    public int Column;
    public int Width;
    public string Format;
    public HorizontalAlignment Alignment = HorizontalAlignment.Left;
  }
}