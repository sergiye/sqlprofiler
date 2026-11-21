using sergiye.Common;
using System.Windows.Forms;

namespace SqlProfiler {
  class ListViewNF : ListViewExtended {
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

    protected override void OnNotifyMessage(Message m) {
      //Filter out the WM_ERASEBKGND message
      if (m.Msg != 0x14) {
        base.OnNotifyMessage(m);
      }
    }
  }
}
