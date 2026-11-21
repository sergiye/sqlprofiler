using sergiye.Common;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const int MaxHighlightedMatches = 300;

    private ToolStrip _searchStrip;
    private ToolStripTextBox _searchBox;
    private ToolStripButton _tbMatchCase;
    private ToolStripButton _tbWholeWord;
    private ToolStripLabel _slSearchCount;

    /// <summary>The search bar under the main toolbar: it finds while the text is typed and highlights the matches in the text panel.</summary>
    private void InitQuickSearch() {
      _searchBox = new ToolStripTextBox {AutoSize = false, Width = 300, ToolTipText = "Enter - next, Shift+Enter - previous, Esc - close"};
      _searchBox.TextChanged += (_, _) => QuickSearch(true, true);
      _searchBox.KeyDown += SearchBox_KeyDown;

      _tbMatchCase = new ToolStripButton("Aa") {CheckOnClick = true, ToolTipText = "Match case", Checked = MatchCase};
      _tbMatchCase.CheckedChanged += (_, _) => {
        MatchCase = _tbMatchCase.Checked;
        QuickSearch(true, true);
      };
      _tbWholeWord = new ToolStripButton("W") {CheckOnClick = true, ToolTipText = "Whole word", Checked = WholeWord};
      _tbWholeWord.CheckedChanged += (_, _) => {
        WholeWord = _tbWholeWord.Checked;
        QuickSearch(true, true);
      };

      _slSearchCount = new ToolStripLabel {AutoSize = false, Width = 110, TextAlign = ContentAlignment.MiddleLeft};

      _searchStrip = new ToolStrip {GripStyle = ToolStripGripStyle.Hidden, Dock = DockStyle.Top, Visible = false};
      _searchStrip.Items.AddRange(new ToolStripItem[] {
        new ToolStripLabel("Find"),
        _searchBox,
        new ToolStripButton("▲", null, (_, _) => QuickSearch(false, false)) {ToolTipText = "Previous (Shift+F3)"},
        new ToolStripButton("▼", null, (_, _) => QuickSearch(true, false)) {ToolTipText = "Next (F3)"},
        _tbMatchCase,
        _tbWholeWord,
        _slSearchCount,
        new ToolStripButton("✕", null, (_, _) => HideQuickSearch()) {Alignment = ToolStripItemAlignment.Right, ToolTipText = "Close (Esc)"}
      });

      Controls.Add(_searchStrip);
      // the strip is docked right after the main toolbar and before the list
      Controls.SetChildIndex(_searchStrip, Controls.GetChildIndex(splitContainer1) + 1);
    }

    private void ShowQuickSearch() {
      _searchStrip.Visible = true;
      // a text selected in the bottom panel is a natural thing to search for
      if (reTextData.SelectionLength > 0 && !reTextData.SelectedText.Contains("\n"))
        _searchBox.Text = reTextData.SelectedText;
      _searchBox.Focus();
      _searchBox.SelectAll();
    }

    private void HideQuickSearch() {
      _searchStrip.Visible = false;
      _slSearchCount.Text = "";
      ClearSearchHighlight();
      lvEvents.Focus();
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.Enter:
          QuickSearch(!e.Shift, false);
          e.SuppressKeyPress = true;
          break;
        case Keys.Escape:
          HideQuickSearch();
          e.SuppressKeyPress = true;
          break;
      }
    }

    /// <summary>Selects the next/previous match; while typing the match is searched from the current row, so it does not jump away.</summary>
    private void QuickSearch(bool forwards, bool fromCurrentRow) {
      var pattern = _searchBox.Text;
      LastPattern = pattern;
      if (pattern.Length == 0) {
        _slSearchCount.Text = "";
        ClearSearchHighlight();
        return;
      }

      var current = FocusedIndex;
      if (fromCurrentRow) current += forwards ? -1 : 1;
      var index = EventSearch.Find(_rows, pattern, MatchCase, WholeWord, forwards, true, Math.Max(current, -1));
      if (index < 0) {
        _slSearchCount.Text = "not found";
        ClearSearchHighlight();
        return;
      }

      var matches = EventSearch.FindAll(_rows, pattern, MatchCase, WholeWord);
      _slSearchCount.Text = $"{matches.IndexOf(index) + 1} of {matches.Count}";
      SelectFoundRow(index);
    }

    /// <summary>Marks the occurrences of the search text in the text panel.</summary>
    private void HighlightSearchInSource() {
      if (_searchStrip is null || !_searchStrip.Visible || string.IsNullOrEmpty(_searchBox.Text)) return;

      var escaped = Regex.Escape(_searchBox.Text);
      var regex = new Regex(WholeWord ? "\\b" + escaped + "\\b" : escaped, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
      var color = ThemeDraw.Mix(Theme.Current.BackgroundColor, Theme.Current.WarnColor, 0.6);
      var count = 0;
      foreach (Match match in regex.Matches(reTextData.Text)) {
        if (++count > MaxHighlightedMatches) break;
        reTextData.Select(match.Index, match.Length);
        reTextData.SelectionBackColor = color;
      }

      reTextData.Select(0, 0);
    }

    private void ClearSearchHighlight() {
      // the text panel is rebuilt from the selected rows, which removes the marks
      _prevSelectedIndices = null;
      UpdateSourceBox();
    }
  }
}
