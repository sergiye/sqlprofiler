using System;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private PresetStore _presets;
    private ToolStripDropDownButton _ddPresets;

    /// <summary>The "Presets" button: named sets of events, columns and filters that can be applied before the trace is started.</summary>
    private void InitPresets() {
      _presets = new PresetStore(_settings);
      _ddPresets = new ToolStripDropDownButton("Presets") {ToolTipText = "Saved sets of events, columns and filters", ShowDropDownArrow = true};
      _ddPresets.DropDownOpening += (_, _) => FillPresetsMenu();
      toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(tbFilterEvents) + 1, _ddPresets);
    }

    private void FillPresetsMenu() {
      _ddPresets.DropDownItems.Clear();
      var canApply = _session.State == ProfilingStateEnum.psStopped;
      var names = _presets.Names;
      foreach (var name in names) {
        var presetName = name;
        _ddPresets.DropDownItems.Add(new ToolStripMenuItem(presetName, null, (_, _) => ApplyPreset(presetName)) {
          Enabled = canApply,
          ToolTipText = canApply ? "Use these events, columns and filters for the next trace" : "Stop the trace first"
        });
      }

      if (names.Count > 0)
        _ddPresets.DropDownItems.Add(new ToolStripSeparator());
      _ddPresets.DropDownItems.Add(new ToolStripMenuItem("Save current settings as...", null, (_, _) => SaveCurrentPreset()));

      var delete = new ToolStripMenuItem("Delete") {Enabled = names.Count > 0};
      foreach (var name in names) {
        var presetName = name;
        delete.DropDownItems.Add(new ToolStripMenuItem(presetName, null, (_, _) => DeletePreset(presetName)));
      }

      _ddPresets.DropDownItems.Add(delete);
    }

    private void ApplyPreset(string name) {
      var preset = _presets.Load(name);
      if (preset is null) {
        MessageBox.Show($"The preset '{name}' cannot be read.", "Presets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      _traceSettings = preset;
      InitGridColumns();
      UpdateEventsStatus();
      _ddPresets.ToolTipText = $"Saved sets of events, columns and filters (last used: {name})";
    }

    private void SaveCurrentPreset() {
      var name = PromptDialog.Ask(this, "Save preset", "Name of the preset:");
      if (name is null) return;
      name = PresetStore.NormalizeName(name);
      if (name.Length == 0) return;

      if (_presets.Exists(name) &&
          MessageBox.Show($"The preset '{name}' already exists. Replace it?", "Presets", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        return;

      _presets.Save(name, _traceSettings);
    }

    private void DeletePreset(string name) {
      if (MessageBox.Show($"Delete the preset '{name}'?", "Presets", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        _presets.Delete(name);
    }
  }
}
