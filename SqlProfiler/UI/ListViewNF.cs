using sergiye.Common;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SqlProfiler {
  class ListViewNF : ListViewExtended {
    private const int LvmGetHeader = 0x101F;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    public ListViewNF() {
      //Activate double buffering
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

      //Enable the OnNotifyMessage event so we get a chance to filter out
      // Windows messages before they get to the form's WndProc
      this.SetStyle(ControlStyles.EnableNotifyMessage, true);
    }

    /// <summary>A click on a new column sorts it ascending, a click on the sorted column reverses the direction.</summary>
    public void ToggleSort(int column) {
      SortOrder = column == SortColumn && SortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
      SortColumn = column;
    }

    /// <summary>Whether the point (in the coordinates of the list) is over the column header.</summary>
    public bool IsOverHeader(Point point) {
      if (!IsHandleCreated) return false;
      var header = SendMessage(Handle, LvmGetHeader, IntPtr.Zero, IntPtr.Zero);
      if (header == IntPtr.Zero || !GetWindowRect(header, out var rect)) return false;
      var topLeft = PointToClient(new Point(rect.left, rect.top));
      var bottomRight = PointToClient(new Point(rect.right, rect.bottom));
      return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y).Contains(point);
    }

    protected override void OnNotifyMessage(Message m) {
      //Filter out the WM_ERASEBKGND message
      if (m.Msg != 0x14) {
        base.OnNotifyMessage(m);
      }
    }
  }
}
