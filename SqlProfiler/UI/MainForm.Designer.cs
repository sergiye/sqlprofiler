﻿namespace SqlProfiler {
  partial class MainForm {
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
      this.components = new System.ComponentModel.Container();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.slEPS = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.tbClear = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.tbStart = new System.Windows.Forms.ToolStripSplitButton();
      this.tbRun = new System.Windows.Forms.ToolStripMenuItem();
      this.tbRunWithFilters = new System.Windows.Forms.ToolStripMenuItem();
      this.tbPause = new System.Windows.Forms.ToolStripButton();
      this.tbStop = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
      this.tbFilterEvents = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
      this.edServer = new System.Windows.Forms.ToolStripTextBox();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.tbAuth = new System.Windows.Forms.ToolStripComboBox();
      this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
      this.edUser = new System.Windows.Forms.ToolStripTextBox();
      this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
      this.edPassword = new System.Windows.Forms.ToolStripTextBox();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.reTextData = new System.Windows.Forms.RichTextBox();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.copyAllToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copySelectedToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.copyToXlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mainMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.storeSeiingsInFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.startTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mnRun = new System.Windows.Forms.ToolStripMenuItem();
      this.mnRunWithFilters = new System.Windows.Forms.ToolStripMenuItem();
      this.pauseTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.stopTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.extractAllEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.extractSelectedEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyAllForExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
      this.saveAllEventsToExcelXmlFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
      this.clearTraceWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.keepSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.autoScrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.stayOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.themeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.filterCapturedEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.clearCapturedFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.siteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip1.SuspendLayout();
      this.toolStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.contextMenu.SuspendLayout();
      this.mainMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // statusStrip1
      // 
      this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slEPS});
      this.statusStrip1.Location = new System.Drawing.Point(0, 802);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
      this.statusStrip1.Size = new System.Drawing.Size(1468, 22);
      this.statusStrip1.TabIndex = 0;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // slEPS
      // 
      this.slEPS.Name = "slEPS";
      this.slEPS.Size = new System.Drawing.Size(0, 15);
      // 
      // toolStrip1
      // 
      this.toolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbClear,
            this.toolStripSeparator1,
            this.tbStart,
            this.tbPause,
            this.tbStop,
            this.toolStripSeparator7,
            this.tbFilterEvents,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.edServer,
            this.toolStripSeparator4,
            this.tbAuth,
            this.toolStripLabel2,
            this.edUser,
            this.toolStripLabel3,
            this.edPassword,
            this.toolStripSeparator3});
      this.toolStrip1.Location = new System.Drawing.Point(0, 36);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.toolStrip1.Size = new System.Drawing.Size(1468, 41);
      this.toolStrip1.TabIndex = 1;
      // 
      // tbClear
      // 
      this.tbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tbClear.Image = global::SqlProfiler.Properties.Resources.imClear;
      this.tbClear.ImageTransparentColor = System.Drawing.Color.Silver;
      this.tbClear.Name = "tbClear";
      this.tbClear.Size = new System.Drawing.Size(36, 36);
      this.tbClear.Text = "Clear trace";
      this.tbClear.ToolTipText = "Clear trace\r\nCtrl+Shift+Del";
      this.tbClear.Click += new System.EventHandler(this.tbClear_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 41);
      // 
      // tbStart
      // 
      this.tbStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tbStart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbRun,
            this.tbRunWithFilters});
      this.tbStart.Image = global::SqlProfiler.Properties.Resources.imStart;
      this.tbStart.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.tbStart.Name = "tbStart";
      this.tbStart.Size = new System.Drawing.Size(53, 36);
      this.tbStart.Text = "Start trace";
      // 
      // tbRun
      // 
      this.tbRun.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
      this.tbRun.Name = "tbRun";
      this.tbRun.Size = new System.Drawing.Size(231, 34);
      this.tbRun.Text = "Run";
      this.tbRun.Click += new System.EventHandler(this.tbStart_Click);
      // 
      // tbRunWithFilters
      // 
      this.tbRunWithFilters.Name = "tbRunWithFilters";
      this.tbRunWithFilters.Size = new System.Drawing.Size(231, 34);
      this.tbRunWithFilters.Text = "Run with filters";
      this.tbRunWithFilters.Click += new System.EventHandler(this.tbRunWithFilters_Click);
      // 
      // tbPause
      // 
      this.tbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tbPause.Image = global::SqlProfiler.Properties.Resources.imPause;
      this.tbPause.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.tbPause.Name = "tbPause";
      this.tbPause.Size = new System.Drawing.Size(36, 36);
      this.tbPause.Text = "Pause trace";
      this.tbPause.Click += new System.EventHandler(this.tbPause_Click);
      // 
      // tbStop
      // 
      this.tbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tbStop.Image = global::SqlProfiler.Properties.Resources.imStop;
      this.tbStop.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.tbStop.Name = "tbStop";
      this.tbStop.Size = new System.Drawing.Size(36, 36);
      this.tbStop.Text = "Stop trace";
      this.tbStop.Click += new System.EventHandler(this.tbStop_Click);
      // 
      // toolStripSeparator7
      // 
      this.toolStripSeparator7.Name = "toolStripSeparator7";
      this.toolStripSeparator7.Size = new System.Drawing.Size(6, 41);
      // 
      // tbFilterEvents
      // 
      this.tbFilterEvents.CheckOnClick = true;
      this.tbFilterEvents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tbFilterEvents.Image = global::SqlProfiler.Properties.Resources.filter;
      this.tbFilterEvents.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.tbFilterEvents.Name = "tbFilterEvents";
      this.tbFilterEvents.Size = new System.Drawing.Size(36, 36);
      this.tbFilterEvents.Text = "Filter Captured Events ";
      this.tbFilterEvents.Click += new System.EventHandler(this.tbFilterEvents_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 41);
      // 
      // toolStripLabel1
      // 
      this.toolStripLabel1.Name = "toolStripLabel1";
      this.toolStripLabel1.Size = new System.Drawing.Size(61, 36);
      this.toolStripLabel1.Text = "Server";
      // 
      // edServer
      // 
      this.edServer.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.edServer.Name = "edServer";
      this.edServer.Size = new System.Drawing.Size(148, 41);
      this.edServer.TextChanged += new System.EventHandler(this.edServer_TextChanged);
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(6, 41);
      // 
      // tbAuth
      // 
      this.tbAuth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.tbAuth.Items.AddRange(new object[] {
            "Windows auth",
            "SQL Server auth"});
      this.tbAuth.Name = "tbAuth";
      this.tbAuth.Size = new System.Drawing.Size(180, 41);
      this.tbAuth.SelectedIndexChanged += new System.EventHandler(this.tbAuth_SelectedIndexChanged);
      // 
      // toolStripLabel2
      // 
      this.toolStripLabel2.Name = "toolStripLabel2";
      this.toolStripLabel2.Size = new System.Drawing.Size(47, 36);
      this.toolStripLabel2.Text = "User";
      // 
      // edUser
      // 
      this.edUser.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.edUser.Name = "edUser";
      this.edUser.Size = new System.Drawing.Size(148, 41);
      this.edUser.TextChanged += new System.EventHandler(this.edUser_TextChanged);
      // 
      // toolStripLabel3
      // 
      this.toolStripLabel3.Name = "toolStripLabel3";
      this.toolStripLabel3.Size = new System.Drawing.Size(87, 36);
      this.toolStripLabel3.Text = "Password";
      // 
      // edPassword
      // 
      this.edPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.edPassword.Name = "edPassword";
      this.edPassword.Size = new System.Drawing.Size(148, 41);
      this.edPassword.TextChanged += new System.EventHandler(this.edPassword_TextChanged);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 41);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 77);
      this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.reTextData);
      this.splitContainer1.Size = new System.Drawing.Size(1468, 725);
      this.splitContainer1.SplitterDistance = 459;
      this.splitContainer1.SplitterWidth = 7;
      this.splitContainer1.TabIndex = 4;
      // 
      // reTextData
      // 
      this.reTextData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.reTextData.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.reTextData.Location = new System.Drawing.Point(0, 0);
      this.reTextData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.reTextData.Name = "reTextData";
      this.reTextData.ReadOnly = true;
      this.reTextData.Size = new System.Drawing.Size(1468, 259);
      this.reTextData.TabIndex = 4;
      this.reTextData.Text = "";
      // 
      // timer1
      // 
      this.timer1.Interval = 250;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // contextMenu
      // 
      this.contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAllToClipboardToolStripMenuItem,
            this.copySelectedToClipboardToolStripMenuItem,
            this.toolStripMenuItem1,
            this.copyToXlsToolStripMenuItem});
      this.contextMenu.Name = "contextMenuStrip1";
      this.contextMenu.Size = new System.Drawing.Size(354, 106);
      // 
      // copyAllToClipboardToolStripMenuItem
      // 
      this.copyAllToClipboardToolStripMenuItem.Name = "copyAllToClipboardToolStripMenuItem";
      this.copyAllToClipboardToolStripMenuItem.Size = new System.Drawing.Size(353, 32);
      this.copyAllToClipboardToolStripMenuItem.Text = "Copy all events to clipboard";
      this.copyAllToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyAllToClipboardToolStripMenuItem_Click);
      // 
      // copySelectedToClipboardToolStripMenuItem
      // 
      this.copySelectedToClipboardToolStripMenuItem.Name = "copySelectedToClipboardToolStripMenuItem";
      this.copySelectedToClipboardToolStripMenuItem.Size = new System.Drawing.Size(353, 32);
      this.copySelectedToClipboardToolStripMenuItem.Text = "Copy selected events to clipboard";
      this.copySelectedToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copySelectedToClipboardToolStripMenuItem_Click);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(350, 6);
      // 
      // copyToXlsToolStripMenuItem
      // 
      this.copyToXlsToolStripMenuItem.Name = "copyToXlsToolStripMenuItem";
      this.copyToXlsToolStripMenuItem.Size = new System.Drawing.Size(353, 32);
      this.copyToXlsToolStripMenuItem.Text = "Copy all for Excel";
      this.copyToXlsToolStripMenuItem.Click += new System.EventHandler(this.copyToXlsToolStripMenuItem_Click);
      // 
      // mainMenu
      // 
      this.mainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.mainMenu.Location = new System.Drawing.Point(0, 0);
      this.mainMenu.Name = "mainMenu";
      this.mainMenu.Size = new System.Drawing.Size(1468, 36);
      this.mainMenu.TabIndex = 5;
      this.mainMenu.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.storeSeiingsInFileToolStripMenuItem,
            this.startTraceToolStripMenuItem,
            this.pauseTraceToolStripMenuItem,
            this.stopTraceToolStripMenuItem,
            this.toolStripMenuItem3,
            this.extractAllEventsToolStripMenuItem,
            this.extractSelectedEventsToolStripMenuItem,
            this.copyAllForExcelToolStripMenuItem,
            this.toolStripSeparator6,
            this.saveAllEventsToExcelXmlFileToolStripMenuItem,
            this.toolStripMenuItem5,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 30);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // storeSeiingsInFileToolStripMenuItem
      // 
      this.storeSeiingsInFileToolStripMenuItem.Name = "storeSeiingsInFileToolStripMenuItem";
      this.storeSeiingsInFileToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.storeSeiingsInFileToolStripMenuItem.Text = "Store settings in file";
      // 
      // startTraceToolStripMenuItem
      // 
      this.startTraceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnRun,
            this.mnRunWithFilters});
      this.startTraceToolStripMenuItem.Name = "startTraceToolStripMenuItem";
      this.startTraceToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.startTraceToolStripMenuItem.Text = "&Start trace";
      // 
      // mnRun
      // 
      this.mnRun.Name = "mnRun";
      this.mnRun.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
      this.mnRun.Size = new System.Drawing.Size(231, 34);
      this.mnRun.Text = "Run";
      this.mnRun.Click += new System.EventHandler(this.tbStart_Click);
      // 
      // mnRunWithFilters
      // 
      this.mnRunWithFilters.Name = "mnRunWithFilters";
      this.mnRunWithFilters.Size = new System.Drawing.Size(231, 34);
      this.mnRunWithFilters.Text = "Run with filters";
      this.mnRunWithFilters.Click += new System.EventHandler(this.tbRunWithFilters_Click);
      // 
      // pauseTraceToolStripMenuItem
      // 
      this.pauseTraceToolStripMenuItem.Name = "pauseTraceToolStripMenuItem";
      this.pauseTraceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
      this.pauseTraceToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.pauseTraceToolStripMenuItem.Text = "&Pause trace";
      this.pauseTraceToolStripMenuItem.Click += new System.EventHandler(this.pauseTraceToolStripMenuItem_Click);
      // 
      // stopTraceToolStripMenuItem
      // 
      this.stopTraceToolStripMenuItem.Name = "stopTraceToolStripMenuItem";
      this.stopTraceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
      this.stopTraceToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.stopTraceToolStripMenuItem.Text = "S&top trace";
      this.stopTraceToolStripMenuItem.Click += new System.EventHandler(this.stopTraceToolStripMenuItem_Click);
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(488, 6);
      // 
      // extractAllEventsToolStripMenuItem
      // 
      this.extractAllEventsToolStripMenuItem.Name = "extractAllEventsToolStripMenuItem";
      this.extractAllEventsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
      this.extractAllEventsToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.extractAllEventsToolStripMenuItem.Text = "Copy all events to clipboard";
      this.extractAllEventsToolStripMenuItem.Click += new System.EventHandler(this.extractAllEventsToolStripMenuItem_Click);
      // 
      // extractSelectedEventsToolStripMenuItem
      // 
      this.extractSelectedEventsToolStripMenuItem.Name = "extractSelectedEventsToolStripMenuItem";
      this.extractSelectedEventsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.C)));
      this.extractSelectedEventsToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.extractSelectedEventsToolStripMenuItem.Text = "Copy selected events to clipboard";
      this.extractSelectedEventsToolStripMenuItem.Click += new System.EventHandler(this.extractSelectedEventsToolStripMenuItem_Click);
      // 
      // copyAllForExcelToolStripMenuItem
      // 
      this.copyAllForExcelToolStripMenuItem.Name = "copyAllForExcelToolStripMenuItem";
      this.copyAllForExcelToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.copyAllForExcelToolStripMenuItem.Text = "Copy all for Excel";
      this.copyAllForExcelToolStripMenuItem.Click += new System.EventHandler(this.copyToXlsToolStripMenuItem_Click);
      // 
      // toolStripSeparator6
      // 
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new System.Drawing.Size(488, 6);
      // 
      // saveAllEventsToExcelXmlFileToolStripMenuItem
      // 
      this.saveAllEventsToExcelXmlFileToolStripMenuItem.Name = "saveAllEventsToExcelXmlFileToolStripMenuItem";
      this.saveAllEventsToExcelXmlFileToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.saveAllEventsToExcelXmlFileToolStripMenuItem.Text = "Save all events to Excel Xml File";
      this.saveAllEventsToExcelXmlFileToolStripMenuItem.Click += new System.EventHandler(this.saveAllEventsToExcelXmlFileToolStripMenuItem_Click);
      // 
      // toolStripMenuItem5
      // 
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new System.Drawing.Size(488, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(491, 34);
      this.exitToolStripMenuItem.Text = "E&xit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // editToolStripMenuItem
      // 
      this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.toolStripMenuItem4,
            this.clearTraceWindowToolStripMenuItem,
            this.deleteSelectedToolStripMenuItem,
            this.keepSelectedToolStripMenuItem});
      this.editToolStripMenuItem.Name = "editToolStripMenuItem";
      this.editToolStripMenuItem.Size = new System.Drawing.Size(58, 30);
      this.editToolStripMenuItem.Text = "&Edit";
      // 
      // selectAllToolStripMenuItem
      // 
      this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
      this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
      this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(393, 34);
      this.selectAllToolStripMenuItem.Text = "Select all";
      this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
      // 
      // findToolStripMenuItem
      // 
      this.findToolStripMenuItem.Name = "findToolStripMenuItem";
      this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
      this.findToolStripMenuItem.Size = new System.Drawing.Size(393, 34);
      this.findToolStripMenuItem.Text = "Find...";
      this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
      // 
      // findNextToolStripMenuItem
      // 
      this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
      this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
      this.findNextToolStripMenuItem.Size = new System.Drawing.Size(393, 34);
      this.findNextToolStripMenuItem.Text = "Find next";
      this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
      // 
      // toolStripMenuItem4
      // 
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(390, 6);
      // 
      // clearTraceWindowToolStripMenuItem
      // 
      this.clearTraceWindowToolStripMenuItem.Name = "clearTraceWindowToolStripMenuItem";
      this.clearTraceWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Delete)));
      this.clearTraceWindowToolStripMenuItem.Size = new System.Drawing.Size(393, 34);
      this.clearTraceWindowToolStripMenuItem.Text = "Clear Trace Window";
      this.clearTraceWindowToolStripMenuItem.Click += new System.EventHandler(this.clearTraceWindowToolStripMenuItem_Click);
      // 
      // deleteSelectedToolStripMenuItem
      // 
      this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
      this.deleteSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
      this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(393, 34);
      this.deleteSelectedToolStripMenuItem.Text = "Delete selected";
      this.deleteSelectedToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedToolStripMenuItem_Click);
      // 
      // keepSelectedToolStripMenuItem
      // 
      this.keepSelectedToolStripMenuItem.Name = "keepSelectedToolStripMenuItem";
      this.keepSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Delete)));
      this.keepSelectedToolStripMenuItem.Size = new System.Drawing.Size(393, 34);
      this.keepSelectedToolStripMenuItem.Text = "Keep selected";
      this.keepSelectedToolStripMenuItem.Click += new System.EventHandler(this.keepSelectedToolStripMenuItem_Click);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoScrollToolStripMenuItem,
            this.stayOnTopToolStripMenuItem,
            this.themeMenuItem});
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(65, 30);
      this.viewToolStripMenuItem.Text = "View";
      // 
      // autoScrollToolStripMenuItem
      // 
      this.autoScrollToolStripMenuItem.Name = "autoScrollToolStripMenuItem";
      this.autoScrollToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      this.autoScrollToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
      this.autoScrollToolStripMenuItem.Text = "Auto scroll";
      // 
      // stayOnTopToolStripMenuItem
      // 
      this.stayOnTopToolStripMenuItem.Name = "stayOnTopToolStripMenuItem";
      this.stayOnTopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
      this.stayOnTopToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
      this.stayOnTopToolStripMenuItem.Text = "Stay on top";
      this.stayOnTopToolStripMenuItem.Click += new System.EventHandler(this.stayOnTop_Click);
      // 
      // themeMenuItem
      // 
      this.themeMenuItem.Name = "themeMenuItem";
      this.themeMenuItem.Size = new System.Drawing.Size(270, 34);
      this.themeMenuItem.Text = "Themes";
      // 
      // filterToolStripMenuItem
      // 
      this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterCapturedEventsToolStripMenuItem,
            this.clearCapturedFiltersToolStripMenuItem});
      this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
      this.filterToolStripMenuItem.Size = new System.Drawing.Size(66, 30);
      this.filterToolStripMenuItem.Text = "Filter";
      // 
      // filterCapturedEventsToolStripMenuItem
      // 
      this.filterCapturedEventsToolStripMenuItem.Name = "filterCapturedEventsToolStripMenuItem";
      this.filterCapturedEventsToolStripMenuItem.Size = new System.Drawing.Size(286, 34);
      this.filterCapturedEventsToolStripMenuItem.Text = "Filter Captured Events";
      this.filterCapturedEventsToolStripMenuItem.Click += new System.EventHandler(this.filterCapturedEventsToolStripMenuItem_Click);
      // 
      // clearCapturedFiltersToolStripMenuItem
      // 
      this.clearCapturedFiltersToolStripMenuItem.Name = "clearCapturedFiltersToolStripMenuItem";
      this.clearCapturedFiltersToolStripMenuItem.Size = new System.Drawing.Size(286, 34);
      this.clearCapturedFiltersToolStripMenuItem.Text = "Clear Captured Filters";
      this.clearCapturedFiltersToolStripMenuItem.Click += new System.EventHandler(this.clearCapturedFiltersToolStripMenuItem_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.siteToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.aboutToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(65, 30);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // siteToolStripMenuItem
      // 
      this.siteToolStripMenuItem.Name = "siteToolStripMenuItem";
      this.siteToolStripMenuItem.Size = new System.Drawing.Size(258, 34);
      this.siteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1;
      this.siteToolStripMenuItem.Text = "Site";
      this.siteToolStripMenuItem.Click += new System.EventHandler(this.siteToolStripMenuItem_Click);
      // 
      // checkForUpdatesToolStripMenuItem
      // 
      this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
      this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(258, 34);
      this.checkForUpdatesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U;
      this.checkForUpdatesToolStripMenuItem.Text = "Check for updates";
      this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(258, 34);
      this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.mnAbout_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1468, 824);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.mainMenu);
      this.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.MainMenuStrip = this.mainMenu;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "MainForm";
      this.Text = "SqlProfiler v1.0";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.contextMenu.ResumeLayout(false);
      this.mainMenu.ResumeLayout(false);
      this.mainMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton tbStop;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.RichTextBox reTextData;
    private System.Windows.Forms.ToolStripButton tbPause;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    private System.Windows.Forms.ToolStripTextBox edServer;
    private System.Windows.Forms.ToolStripLabel toolStripLabel2;
    private System.Windows.Forms.ToolStripTextBox edUser;
    private System.Windows.Forms.ToolStripLabel toolStripLabel3;
    private System.Windows.Forms.ToolStripTextBox edPassword;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripComboBox tbAuth;
    private System.Windows.Forms.ToolStripButton tbClear;
    private System.Windows.Forms.ContextMenuStrip contextMenu;
    private System.Windows.Forms.ToolStripMenuItem copyAllToClipboardToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem copySelectedToClipboardToolStripMenuItem;
    private System.Windows.Forms.MenuStrip mainMenu;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem startTraceToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem pauseTraceToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem stopTraceToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem clearTraceWindowToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    private System.Windows.Forms.ToolStripMenuItem extractAllEventsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem extractSelectedEventsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
    private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripSplitButton tbStart;
    private System.Windows.Forms.ToolStripMenuItem tbRun;
    private System.Windows.Forms.ToolStripMenuItem tbRunWithFilters;
    private System.Windows.Forms.ToolStripMenuItem mnRun;
    private System.Windows.Forms.ToolStripMenuItem mnRunWithFilters;
    private System.Windows.Forms.ToolStripStatusLabel slEPS;
    private System.Windows.Forms.ToolStripMenuItem copyToXlsToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem copyAllForExcelToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem stayOnTopToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem themeMenuItem;
    private System.Windows.Forms.ToolStripMenuItem deleteSelectedToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem keepSelectedToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem filterCapturedEventsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem clearCapturedFiltersToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    private System.Windows.Forms.ToolStripMenuItem saveAllEventsToExcelXmlFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
    private System.Windows.Forms.ToolStripButton tbFilterEvents;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem siteToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem storeSeiingsInFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem autoScrollToolStripMenuItem;
  }
}