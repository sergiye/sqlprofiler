using sergiye.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SqlProfiler {
  /// <summary>A small dialog that asks for one line of text.</summary>
  internal sealed class PromptDialog : Form {
    private readonly TextBox _input;

    private PromptDialog(string title, string prompt, string value) {
      Text = title;
      FormBorderStyle = FormBorderStyle.FixedDialog;
      StartPosition = FormStartPosition.CenterParent;
      MinimizeBox = false;
      MaximizeBox = false;
      ShowInTaskbar = false;
      ClientSize = new Size(360, 110);
      Font = SystemFonts.MessageBoxFont;

      var label = new Label {Text = prompt, Left = 12, Top = 12, Width = 336, AutoSize = false, Height = 20};
      _input = new TextBox {Left = 12, Top = 36, Width = 336, Text = value};
      var ok = new Button {Text = "OK", Left = 192, Top = 72, Width = 75, DialogResult = DialogResult.OK};
      var cancel = new Button {Text = "Cancel", Left = 273, Top = 72, Width = 75, DialogResult = DialogResult.Cancel};
      Controls.AddRange(new Control[] {label, _input, ok, cancel});
      AcceptButton = ok;
      CancelButton = cancel;
      ThemeSupport.Attach(this);
    }

    /// <summary>Returns the entered text, or null if the dialog was cancelled.</summary>
    public static string Ask(IWin32Window owner, string title, string prompt, string value = "") {
      using var dialog = new PromptDialog(title, prompt, value);
      dialog.Shown += (_, _) => dialog._input.SelectAll();
      return dialog.ShowDialog(owner) == DialogResult.OK ? dialog._input.Text : null;
    }
  }
}
