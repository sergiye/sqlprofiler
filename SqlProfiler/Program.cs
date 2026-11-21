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
      Application.ThreadException += (_, e) => {
        AppLogger.Error("Unhandled UI exception", e.Exception);
        MessageBox.Show($"{e.Exception.Message}\r\n\r\nDetails are written to {AppLogger.FilePath}", "Error",
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      };
      AppLogger.Info($"Started, version {Updater.CurrentVersion}");
      Application.Run(new MainForm());
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      var text = e.ExceptionObject is Exception ex ? ex.ToString() : e.ExceptionObject?.ToString();
      AppLogger.Error("Unhandled exception: " + text);
      MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}