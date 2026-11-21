using System;
using System.IO;
using System.Text;

namespace SqlProfiler {
  /// <summary>Writes errors and important events to %LocalAppData%\SqlProfiler\SqlProfiler.log. Never throws and never logs captured SQL text or passwords.</summary>
  internal static class AppLogger {
    private const long MaxFileSize = 1024 * 1024;
    private static readonly object Sync = new();

    public static string FilePath { get; } = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SqlProfiler", "SqlProfiler.log");

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
          Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
          var file = new FileInfo(FilePath);
          if (file.Exists && file.Length > MaxFileSize) {
            File.Copy(FilePath, FilePath + ".old", true);
            File.Delete(FilePath);
          }

          File.AppendAllText(FilePath, text.ToString(), Encoding.UTF8);
        }
      }
      catch {
        // logging must not break the application
      }
    }
  }
}
