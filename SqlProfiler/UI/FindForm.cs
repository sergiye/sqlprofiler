using sergiye.Common;
using System;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class FindForm : Form {
    private MainForm mainForm;

    public FindForm(MainForm mainForm) {
      InitializeComponent();
      
      StartPosition = FormStartPosition.Manual;
      Location = new System.Drawing.Point(mainForm.Left + mainForm.Width / 2 - Width / 2, mainForm.Top + mainForm.Height / 2 - Height / 2);

      this.mainForm = mainForm;

      // Set the control values to the last find performed.
      edPattern.Text = mainForm.LastPattern;
      cbxMatchCase.Checked = mainForm.MatchCase;
      cbxWholeWord.Checked = mainForm.WholeWord;
      cbxWrapAround.Checked = mainForm.WrapAround;
      ThemeSupport.Attach(this);

      Shown += (s, e) => {
        edPattern.Focus();
      };

      FormClosing += (s, e) => {
        mainForm.MatchCase = cbxMatchCase.Checked;
        mainForm.WholeWord = cbxWholeWord.Checked;
        mainForm.WrapAround = cbxWrapAround.Checked;
      };
    }

    private void BtnFindNext_Click(object sender, EventArgs e) {
      DoFind(true);
    }

    private void BtnFindPrevious_Click(object sender, EventArgs e) {
      DoFind(false);
    }

    private void DoFind(bool forwards) {
      mainForm.LastPattern = edPattern.Text;
      mainForm.PerformFind(forwards, cbxWrapAround.Checked);
    }
  }
}