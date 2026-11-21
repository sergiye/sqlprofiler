using System;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const string FormatSqlSetting = "FormatSql";

    private const string WrapSourceSetting = "SourceWrap";
    private const string LineNumbersSetting = "SourceLines";
    private const string SourceZoomSetting = "SourceZoom";

    private ToolStrip _sourceStrip;
    private ToolStripButton _tbFormatSql;
    private ToolStripButton _tbWrap;
    private ToolStripButton _tbLineNumbers;
    private LineNumberGutter _gutter;

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

      _tbWrap = new ToolStripButton("Wrap") {
        CheckOnClick = true,
        Checked = _settings.GetValue(WrapSourceSetting, 1) == 1,
        ToolTipText = "Wrap long lines"
      };
      _tbWrap.CheckedChanged += (_, _) => {
        reTextData.WordWrap = _tbWrap.Checked;
        _settings.SetValue(WrapSourceSetting, _tbWrap.Checked ? 1 : 0);
        _gutter?.Invalidate();
      };
      reTextData.WordWrap = _tbWrap.Checked;

      _tbLineNumbers = new ToolStripButton("Lines") {
        CheckOnClick = true,
        Checked = _settings.GetValue(LineNumbersSetting, 0) == 1,
        ToolTipText = "Show line numbers"
      };
      _tbLineNumbers.CheckedChanged += (_, _) => {
        _gutter.Visible = _tbLineNumbers.Checked;
        _settings.SetValue(LineNumbersSetting, _tbLineNumbers.Checked ? 1 : 0);
      };

      var zoomOut = new ToolStripButton("A-") {ToolTipText = "Smaller text (Ctrl+mouse wheel)"};
      zoomOut.Click += (_, _) => ZoomSource(1 / 1.1f);
      var zoomIn = new ToolStripButton("A+") {ToolTipText = "Larger text (Ctrl+mouse wheel)"};
      zoomIn.Click += (_, _) => ZoomSource(1.1f);

      _sourceStrip = new ToolStrip {GripStyle = ToolStripGripStyle.Hidden, Dock = DockStyle.Top};
      _sourceStrip.Items.AddRange(new ToolStripItem[] {_tbFormatSql, copy, new ToolStripSeparator(), _tbWrap, _tbLineNumbers, new ToolStripSeparator(), zoomOut, zoomIn});
      splitContainer1.Panel2.Controls.Add(_sourceStrip);
      // the last control of the collection is docked first, so the toolbar stays above the text box
      _sourceStrip.SendToBack();

      _gutter = new LineNumberGutter(reTextData) {Visible = _tbLineNumbers.Checked};
      splitContainer1.Panel2.Controls.Add(_gutter);
      splitContainer1.Panel2.Controls.SetChildIndex(_gutter, splitContainer1.Panel2.Controls.GetChildIndex(_sourceStrip));

      var zoom = _settings.GetValue(SourceZoomSetting, 100);
      if (zoom >= 50 && zoom <= 400) reTextData.ZoomFactor = zoom / 100f;
    }

    private void ZoomSource(float factor) {
      reTextData.ZoomFactor = Math.Min(4f, Math.Max(0.5f, reTextData.ZoomFactor * factor));
      _gutter?.Invalidate();
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
