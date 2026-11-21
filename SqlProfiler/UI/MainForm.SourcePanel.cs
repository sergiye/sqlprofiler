using System;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const string FormatSqlSetting = "FormatSql";

    private ToolStrip _sourceStrip;
    private ToolStripButton _tbFormatSql;

    /// <summary>A small toolbar above the text of the selected queries: formatting and copying.</summary>
    private void InitSourcePanel() {
      _tbFormatSql = new ToolStripButton("Format SQL") {
        CheckOnClick = true,
        Checked = _settings.GetValue(FormatSqlSetting, 0) == 1,
        ToolTipText = "Show the queries with their clauses on separate lines (only the white space is changed)"
      };
      _tbFormatSql.CheckedChanged += (_, _) => {
        _settings.SetValue(FormatSqlSetting, _tbFormatSql.Checked ? 1 : 0);
        _prevSelectedIndices = null;
        UpdateSourceBox();
      };

      var copy = new ToolStripButton("Copy") {ToolTipText = "Copy the text shown below, or only its selected part"};
      copy.Click += (_, _) => CopySourceText();

      _sourceStrip = new ToolStrip {GripStyle = ToolStripGripStyle.Hidden, Dock = DockStyle.Top};
      _sourceStrip.Items.Add(_tbFormatSql);
      _sourceStrip.Items.Add(copy);
      splitContainer1.Panel2.Controls.Add(_sourceStrip);
      // the last control of the collection is docked first, so the toolbar stays above the text box
      _sourceStrip.SendToBack();
    }

    private string FormatForDisplay(string text) {
      return _tbFormatSql?.Checked == true ? SqlFormatter.Format(text) : text;
    }

    private void CopySourceText() {
      var text = reTextData.SelectionLength > 0 ? reTextData.SelectedText : reTextData.Text;
      if (!string.IsNullOrEmpty(text))
        Clipboard.SetText(text);
    }
  }
}
