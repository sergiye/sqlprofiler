﻿namespace SqlProfiler {
  partial class TraceProperties {
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
      this.btnRun = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnReset = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.edFilters = new System.Windows.Forms.PropertyGrid();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.edEvents = new System.Windows.Forms.PropertyGrid();
      this.panel1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnRun
      // 
      this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRun.Location = new System.Drawing.Point(327, 5);
      this.btnRun.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnRun.Name = "btnRun";
      this.btnRun.Size = new System.Drawing.Size(112, 35);
      this.btnRun.TabIndex = 0;
      this.btnRun.Text = "Run";
      this.btnRun.UseVisualStyleBackColor = true;
      this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnReset);
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Controls.Add(this.btnRun);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 853);
      this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(572, 45);
      this.panel1.TabIndex = 1;
      // 
      // btnReset
      // 
      this.btnReset.Location = new System.Drawing.Point(10, 5);
      this.btnReset.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnReset.Name = "btnReset";
      this.btnReset.Size = new System.Drawing.Size(112, 35);
      this.btnReset.TabIndex = 3;
      this.btnReset.Text = "Reset";
      this.btnReset.UseVisualStyleBackColor = true;
      this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(448, 5);
      this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(112, 35);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(572, 853);
      this.tabControl1.TabIndex = 2;
      // 
      // tabPage1
      // 
      this.tabPage1.BackColor = System.Drawing.Color.Transparent;
      this.tabPage1.Controls.Add(this.edFilters);
      this.tabPage1.Location = new System.Drawing.Point(4, 29);
      this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage1.Size = new System.Drawing.Size(564, 820);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Filters";
      // 
      // edFilters
      // 
      this.edFilters.Dock = System.Windows.Forms.DockStyle.Fill;
      this.edFilters.HelpVisible = false;
      this.edFilters.Location = new System.Drawing.Point(4, 5);
      this.edFilters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.edFilters.Name = "edFilters";
      this.edFilters.PropertySort = System.Windows.Forms.PropertySort.Categorized;
      this.edFilters.SelectedObject = this.btnCancel;
      this.edFilters.Size = new System.Drawing.Size(556, 810);
      this.edFilters.TabIndex = 0;
      this.edFilters.ToolbarVisible = false;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.edEvents);
      this.tabPage2.Location = new System.Drawing.Point(4, 29);
      this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage2.Size = new System.Drawing.Size(564, 820);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Columns and events";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // edEvents
      // 
      this.edEvents.Dock = System.Windows.Forms.DockStyle.Fill;
      this.edEvents.Location = new System.Drawing.Point(4, 5);
      this.edEvents.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.edEvents.Name = "edEvents";
      this.edEvents.SelectedObject = this.btnRun;
      this.edEvents.Size = new System.Drawing.Size(556, 810);
      this.edEvents.TabIndex = 1;
      this.edEvents.ToolbarVisible = false;
      // 
      // TraceProperties
      // 
      this.AcceptButton = this.btnRun;
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(572, 898);
      this.ControlBox = false;
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.panel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "TraceProperties";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Trace properties";
      this.panel1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    internal System.Windows.Forms.PropertyGrid edFilters;
    internal System.Windows.Forms.PropertyGrid edEvents;
    private System.Windows.Forms.Button btnReset;
  }
}