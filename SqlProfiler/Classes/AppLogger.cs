using sergiye.Common;
using System;
using System.IO;
using System.Text;

namespace SqlProfiler {
  /// <summary>Writes errors and important events to SqlProfiler.log next to the executable, so a portable copy leaves nothing elsewhere in the system. Never throws and never logs captured SQL text or passwords.</summary>
  internal static class AppLogger {
    private const long MaxFileSize = 1024 * 1024;
    private static readonly object Sync = new();

    public static string FilePath { get; } = Path.ChangeExtension(Updater.CurrentFileLocation, ".log");

    public static void Info(string message) => Write("INFO", message, null);

    public static void Warn(string message) => Write("WARN", message, null);

    public static void Error(string message, Exception exception = null) => Write("ERROR", message, exception);

    private static void Write(string level, string message, Exception exception) {
      try {
        var text = new StringBuilder()
          .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
          .Append(" [").Append(level).Append("] ").AppendLine(message);
        if (exception is not null)
          text.AppendLine(exception.ToString());

        lock (Sync) {
          var file = new FileInfo(FilePath);
          // the log is a short diagnostic aid: when it grows too big it starts again instead of leaving extra files
          if (file.Exists && file.Length > MaxFileSize)
            File.Delete(FilePath);
          File.AppendAllText(FilePath, text.ToString(), Encoding.UTF8);
        }
      }
      catch {
        // logging must not break the application
      }
    }
  }
}
