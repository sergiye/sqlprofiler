using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private ToolStripMenuItem _miSaveCsv;
    private ToolStripMenuItem _miSaveJson;

    private void InitExportFiles() {
      _miSaveCsv = new ToolStripMenuItem("Save all events as CSV...", null, (_, _) => SaveTable("CSV (*.csv)|*.csv", "csv", EventTableExporter.BuildCsv));
      _miSaveJson = new ToolStripMenuItem("Save all events as JSON...", null, (_, _) => SaveTable("JSON (*.json)|*.json", "json", EventTableExporter.BuildJson));
      var index = fileToolStripMenuItem.DropDownItems.IndexOf(saveAllEventsToExcelXmlFileToolStripMenuItem) + 1;
      fileToolStripMenuItem.DropDownItems.Insert(index, _miSaveCsv);
      fileToolStripMenuItem.DropDownItems.Insert(index + 1, _miSaveJson);
      fileToolStripMenuItem.DropDownOpening += (_, _) => {
        var hasEvents = _rows.Any(r => !r.IsMarker);
        _miSaveCsv.Enabled = hasEvents;
        _miSaveJson.Enabled = hasEvents;
      };
    }

    private void SaveTable(string filter, string extension, Func<System.Collections.Generic.IEnumerable<EventRow>, string> build) {
      var rows = _rows.Where(r => !r.IsMarker).ToList();
      if (rows.Count == 0) return;

      using var dialog = new SaveFileDialog {Filter = filter, DefaultExt = extension, Title = "Save events"};
      if (dialog.ShowDialog(this) != DialogResult.OK) return;
      try {
        // UTF-8 with the byte order mark, so that Excel reads the national characters of a CSV file correctly
        File.WriteAllText(dialog.FileName, build(rows), new UTF8Encoding(true));
        MessageBox.Show($"{rows.Count:#,0} events saved to {dialog.FileName}", "Save events", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception e) {
        AppLogger.Error("Failed to save the events", e);
        MessageBox.Show("Cannot save the events:\r\n" + e.Message, "Save events", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
