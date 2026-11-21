using sergiye.Common;
using System;
using System.Drawing;

namespace SqlProfiler {
  internal static class ColorTools {
    /// <summary>Relative luminance of a color (WCAG).</summary>
    public static double Luminance(Color color) {
      static double Channel(byte value) {
        var v = value / 255.0;
        return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
      }

      return 0.2126 * Channel(color.R) + 0.7152 * Channel(color.G) + 0.0722 * Channel(color.B);
    }

    public static double Contrast(Color first, Color second) {
      var a = Luminance(first);
      var b = Luminance(second);
      return (Math.Max(a, b) + 0.05) / (Math.Min(a, b) + 0.05);
    }

    /// <summary>The color made lighter on a dark background (darker on a light one) until it can be read on it.</summary>
    public static Color EnsureContrast(Color color, Color background, double minRatio = 4.5) {
      if (Contrast(color, background) >= minRatio) return color;

      var target = Luminance(background) < 0.5 ? Color.White : Color.Black;
      for (var part = 0.1; part < 1.0; part += 0.1) {
        var mixed = ThemeDraw.Mix(color, target, part);
        if (Contrast(mixed, background) >= minRatio) return mixed;
      }

      return target;
    }
  }
}
