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
      edPattern.Text = mainForm.lastpattern;
      cbxMatchCase.Checked = mainForm.matchCase;
      cbxWholeWord.Checked = mainForm.wholeWord;
      cbxWrapAround.Checked = mainForm.wrapAround;
      Theme.Current.Apply(this);

      Shown += (s, e) => {
        edPattern.Focus();
      };

      FormClosing += (s, e) => {
        mainForm.matchCase = cbxMatchCase.Checked;
        mainForm.wholeWord = cbxWholeWord.Checked;
        mainForm.wrapAround = cbxWrapAround.Checked;
      };
    }

    private void btnFindNext_Click(object sender, EventArgs e) {
      DoFind(true);
    }

    private void btnFindPrevious_Click(object sender, EventArgs e) {
      DoFind(false);
    }

    private void DoFind(bool forwards) {
      mainForm.lastpattern = edPattern.Text;
      mainForm.PerformFind(forwards, cbxWrapAround.Checked);
    }

    private void edPattern_TextChanged(object sender, EventArgs e) {
      mainForm.lastpos = -1;
    }
  }
}