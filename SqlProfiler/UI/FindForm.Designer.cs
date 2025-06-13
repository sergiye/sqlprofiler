namespace SqlProfiler {
  partial class FindForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }

      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.edPattern = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnFindNext = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.cbxMatchCase = new System.Windows.Forms.CheckBox();
      this.cbxWholeWord = new System.Windows.Forms.CheckBox();
      this.btnFindPrevious = new System.Windows.Forms.Button();
      this.cbxWrapAround = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // edPattern
      // 
      this.edPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edPattern.Location = new System.Drawing.Point(6, 31);
      this.edPattern.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.edPattern.Name = "edPattern";
      this.edPattern.Size = new System.Drawing.Size(461, 26);
      this.edPattern.TabIndex = 1;
      this.edPattern.TextChanged += new System.EventHandler(this.edPattern_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(4, 5);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(82, 20);
      this.label1.TabIndex = 0;
      this.label1.Text = "&Find what:";
      // 
      // btnFindNext
      // 
      this.btnFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFindNext.Location = new System.Drawing.Point(477, 29);
      this.btnFindNext.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnFindNext.Name = "btnFindNext";
      this.btnFindNext.Size = new System.Drawing.Size(117, 35);
      this.btnFindNext.TabIndex = 5;
      this.btnFindNext.Text = "Find &next";
      this.btnFindNext.UseVisualStyleBackColor = true;
      this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(477, 133);
      this.btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(117, 35);
      this.btnClose.TabIndex = 7;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      // 
      // chkCase
      // 
      this.cbxMatchCase.AutoSize = true;
      this.cbxMatchCase.Location = new System.Drawing.Point(9, 74);
      this.cbxMatchCase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cbxMatchCase.Name = "chkCase";
      this.cbxMatchCase.Size = new System.Drawing.Size(120, 24);
      this.cbxMatchCase.TabIndex = 2;
      this.cbxMatchCase.Text = "&Match Case";
      this.cbxMatchCase.UseVisualStyleBackColor = true;
      // 
      // chkWholeWord
      // 
      this.cbxWholeWord.AutoSize = true;
      this.cbxWholeWord.Location = new System.Drawing.Point(9, 109);
      this.cbxWholeWord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cbxWholeWord.Name = "chkWholeWord";
      this.cbxWholeWord.Size = new System.Drawing.Size(122, 24);
      this.cbxWholeWord.TabIndex = 3;
      this.cbxWholeWord.Text = "&Whole Word";
      this.cbxWholeWord.UseVisualStyleBackColor = true;
      // 
      // btnFindPrevious
      // 
      this.btnFindPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFindPrevious.Location = new System.Drawing.Point(477, 74);
      this.btnFindPrevious.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnFindPrevious.Name = "btnFindPrevious";
      this.btnFindPrevious.Size = new System.Drawing.Size(117, 35);
      this.btnFindPrevious.TabIndex = 6;
      this.btnFindPrevious.Text = "Find &previous";
      this.btnFindPrevious.UseVisualStyleBackColor = true;
      this.btnFindPrevious.Click += new System.EventHandler(this.btnFindPrevious_Click);
      // 
      // chkWrapAround
      // 
      this.cbxWrapAround.AutoSize = true;
      this.cbxWrapAround.Location = new System.Drawing.Point(9, 145);
      this.cbxWrapAround.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cbxWrapAround.Name = "chkWrapAround";
      this.cbxWrapAround.Size = new System.Drawing.Size(129, 24);
      this.cbxWrapAround.TabIndex = 4;
      this.cbxWrapAround.Text = "Wrap Around";
      this.cbxWrapAround.UseVisualStyleBackColor = true;
      // 
      // FindForm
      // 
      this.AcceptButton = this.btnFindNext;
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(611, 182);
      this.ControlBox = false;
      this.Controls.Add(this.cbxWrapAround);
      this.Controls.Add(this.btnFindPrevious);
      this.Controls.Add(this.cbxWholeWord);
      this.Controls.Add(this.cbxMatchCase);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnFindNext);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.edPattern);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "FindForm";
      this.ShowInTaskbar = false;
      this.Text = "Find";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox edPattern;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnFindNext;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.CheckBox cbxMatchCase;
    private System.Windows.Forms.CheckBox cbxWholeWord;
    private System.Windows.Forms.Button btnFindPrevious;
    private System.Windows.Forms.CheckBox cbxWrapAround;
  }
}