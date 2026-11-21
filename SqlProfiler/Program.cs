using sergiye.Common;
using System;
using System.Windows.Forms;

namespace SqlProfiler {

  static class Program {

    [STAThread]
    static void Main() {

      if (!OSHelper.IsCompatible(true, out var errorMessage, out var fixAction)) {

        if (fixAction is not null) {
          if (MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
            fixAction?.Invoke();
          }
        }
        else {
          MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        Environment.Exit(0);
      }

      Crasher.Listen();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      Application.Run(new MainForm());
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      MessageBox.Show(e.ExceptionObject is Exception ex ? ex.ToString() : e.ExceptionObject?.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}