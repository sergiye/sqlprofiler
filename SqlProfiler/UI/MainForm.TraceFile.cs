using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private ToolStripMenuItem _miSaveTrace;
    private ToolStripMenuItem _miOpenTrace;

    private const string TraceFileFilter = "SqlProfiler trace (*" + TraceFile.Extension + ")|*" + TraceFile.Extension;

    private void InitTraceFile() {
      _miSaveTrace = new ToolStripMenuItem("Save trace to file...", null, (_, _) => SaveTraceToFile());
      _miOpenTrace = new ToolStripMenuItem("Open trace from file...", null, (_, _) => OpenTraceFromFile());
      var index = fileToolStripMenuItem.DropDownItems.IndexOf(saveAllEventsToExcelXmlFileToolStripMenuItem);
      fileToolStripMenuItem.DropDownItems.Insert(index, _miSaveTrace);
      fileToolStripMenuItem.DropDownItems.Insert(index + 1, _miOpenTrace);
      fileToolStripMenuItem.DropDownOpening += (_, _) => {
        _miSaveTrace.Enabled = _rows.Any(r => !r.IsMarker);
        _miOpenTrace.Enabled = _session.State == ProfilingStateEnum.psStopped;
      };
    }

    private void SaveTraceToFile() {
      using var dialog = new SaveFileDialog {Filter = TraceFileFilter, DefaultExt = TraceFile.Extension.TrimStart('.'), Title = "Save trace"};
      if (dialog.ShowDialog(this) != DialogResult.OK) return;
      try {
        var events = _rows.Where(r => !r.IsMarker).Select(r => r.Event).ToList();
        TraceFile.Save(dialog.FileName, events);
        MessageBox.Show($"{events.Count:#,0} events saved to {dialog.FileName}", "Save trace", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception e) {
        AppLogger.Error("Failed to save the trace", e);
        MessageBox.Show("Cannot save the trace:\r\n" + e.Message, "Save trace", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void OpenTraceFromFile() {
      if (_session.State != ProfilingStateEnum.psStopped) {
        MessageBox.Show("Stop the trace first.", "Open trace", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      using var dialog = new OpenFileDialog {Filter = TraceFileFilter, Title = "Open trace"};
      if (dialog.ShowDialog(this) != DialogResult.OK) return;
      try {
        var events = TraceFile.Load(dialog.FileName);
        LoadEvents(events);
        if (_trimmedCount > 0) {
          MessageBox.Show($"The file has {events.Count:#,0} events, the list shows the latest {_rows.Count:#,0}.\r\n" +
                          "Increase 'Maximum events count' in 'Run with filters' to see more of them.",
            "Open trace", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
      catch (Exception e) when (e is IOException || e is InvalidDataException || e is UnauthorizedAccessException) {
        AppLogger.Error("Failed to open the trace", e);
        MessageBox.Show("Cannot open the trace:\r\n" + e.Message, "Open trace", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    /// <summary>Replaces the list with the events of a file.</summary>
    private void LoadEvents(System.Collections.Generic.IReadOnlyCollection<ProfilerEvent> events) {
      ResetCapturedFilter();
      _eventCount = 0;
      _trimmedCount = 0;
      ReplaceRows(Enumerable.Empty<EventRow>());
      AppendEvents(events);
      ApplySort();
    }
  }
}
