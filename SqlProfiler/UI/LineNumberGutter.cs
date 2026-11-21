using sergiye.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SqlProfiler {
  /// <summary>Line numbers to the left of a RichTextBox. A wrapped line has one number, as in a text editor.</summary>
  internal sealed class LineNumberGutter : Control {
    private readonly RichTextBox _rich;

    public LineNumberGutter(RichTextBox rich) {
      _rich = rich;
      SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
      Dock = DockStyle.Left;
      TabStop = false;
      Width = 36;

      rich.VScroll += (_, _) => Invalidate();
      rich.TextChanged += (_, _) => UpdateWidth();
      rich.Resize += (_, _) => Invalidate();
      rich.FontChanged += (_, _) => UpdateWidth();
      // the zoom of the text box changes with Ctrl+mouse wheel, there is no event for it
      rich.MouseWheel += (_, _) => BeginInvoke(new Action(UpdateWidth));
      rich.KeyUp += (_, _) => UpdateWidth();
    }

    private float FontSize => _rich.Font.Size * Math.Max(_rich.ZoomFactor, 0.1f);

    private void UpdateWidth() {
      if (IsDisposed || !IsHandleCreated) return;
      var lines = 1;
      foreach (var c in _rich.Text)
        if (c == '\n') lines++;

      using var font = new Font(_rich.Font.FontFamily, FontSize);
      var width = TextRenderer.MeasureText(new string('9', Math.Max(lines.ToString().Length, 2)), font).Width + 12;
      if (Width != width) Width = width;
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e) {
      var theme = Theme.Current;
      var back = ThemeDraw.Mix(theme.BackgroundColor, theme.ForegroundColor, 0.08);
      var fore = ThemeDraw.Mix(theme.BackgroundColor, theme.ForegroundColor, 0.55);
      e.Graphics.Clear(back);

      var text = _rich.Text;
      if (text.Length == 0) return;

      using var font = new Font(_rich.Font.FontFamily, FontSize);
      var flags = TextFormatFlags.Right | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
      var first = _rich.GetLineFromCharIndex(_rich.GetCharIndexFromPosition(new Point(1, 1)));
      var firstIndex = _rich.GetFirstCharIndexFromLine(first);
      if (firstIndex < 0) return;

      // the number of the logical line that contains the first visible line
      var number = 1;
      for (var i = 0; i < firstIndex && i < text.Length; i++)
        if (text[i] == '\n') number++;

      var firstStartsLine = firstIndex == 0 || text[firstIndex - 1] == '\n';
      for (var line = first; ; line++) {
        var index = _rich.GetFirstCharIndexFromLine(line);
        if (index < 0) break;
        var position = _rich.GetPositionFromCharIndex(index);
        if (position.Y > Height) break;

        var startsLine = index == 0 || text[index - 1] == '\n';
        if (line > first && startsLine) number++;
        if (startsLine || (line == first && firstStartsLine))
          TextRenderer.DrawText(e.Graphics, number.ToString(), font, new Rectangle(0, position.Y, Width - 6, font.Height + 2), fore, flags);
      }
    }
  }
}
