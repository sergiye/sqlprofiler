using sergiye.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SqlProfiler {

  public partial class MainForm : Form {
    
    private RawTraceReader m_Rdr;
    private readonly object m_eventsLock = new();

    private readonly YukonLexer m_Lex = new YukonLexer();
    private SqlConnection m_Conn;
    private CancellationTokenSource m_readerCts;
    private Task m_readerTask;
    private ProfilingStateEnum m_ProfilingState;
    private int m_EventCount;
    private readonly ProfilerEvent m_EventStarted = new();
    private readonly ProfilerEvent m_EventStopped = new();
    private readonly ProfilerEvent m_EventPaused = new();
    private readonly List<ListViewItem> m_Cached = new(1024);
    private readonly List<ListViewItem> m_CachedUnFiltered = new(1024);
    internal string lastpattern = "";
    private ListViewNF lvEvents;
    Queue<ProfilerEvent> m_events = new(10);
    private bool m_autostart;
    private bool dontUpdateSource;
    private Exception m_profilerexception;
    private readonly Queue<PerfInfo> m_perf = new();
    private PerfInfo m_first, m_prev;
    private TraceProperties.TraceSettings m_currentsettings;
    private readonly List<PerfColumn> m_columns = new();
    private int fixedColumnIndex = 1;
    private const int MinFixedColumnWidth = 150;
    internal bool matchCase;
    internal bool wholeWord;
    internal bool wrapAround;
    private readonly PersistentSettings settings;

    public MainForm() {
      settings = new PersistentSettings();
      settings.Load();
      ThemeSupport.Initialize(settings.GetValue("Theme", ""), id => settings.SetValue("Theme", id), "SqlProfiler.themes");

      InitializeComponent();
      tbStart.DefaultItem = tbRun;
      Icon = Icon.ExtractAssociatedIcon(Updater.CurrentFileLocation);
      Text = $"{Updater.ApplicationName} {(Environment.Is64BitProcess ? "x64" : "x32")} - {Updater.CurrentVersion}";
      StartPosition = FormStartPosition.CenterScreen;
      edPassword.TextBox.PasswordChar = '*';

      ThemedWindowTitle.Attach(this, mainMenu, WindowTitleStyle.ThemeColors);

      m_currentsettings = TraceProperties.TraceSettings.GetDefaultSettings();
      ParseCommandLine();
      InitLV();

      var portable = new UserOption("portable", true, storeSettingsInFileToolStripMenuItem, settings);
      portable.Changed += delegate {
        settings.IsPortable = portable.Value;
      };

      var autoScroll = new UserOption("autoScroll", true, autoScrollToolStripMenuItem, settings);

      edServer.Text = settings.GetValue("Server", ".");
      edUser.Text = settings.GetValue("User", "sa");
      edPassword.Text = settings.GetValue("Password", "sa");
      tbAuth.SelectedIndex = string.IsNullOrEmpty(edUser.Text) ? 0 : 1;
      if (m_autostart) RunProfiling(false);
      UpdateButtons();

      MinimumSize = new Size(1100, 666);
      
      Load += (sender, args) => {
        Left = settings.GetValue("Left", Screen.PrimaryScreen.WorkingArea.Left + Width / 2);
        Top = settings.GetValue("Top", Screen.PrimaryScreen.WorkingArea.Top + Height / 2);
        Height = settings.GetValue("Height", Screen.PrimaryScreen.WorkingArea.Height / 2);
        Width = settings.GetValue("Width", Screen.PrimaryScreen.WorkingArea.Width / 2);

        matchCase = settings.GetValue("MatchCase", false);
        wholeWord = settings.GetValue("WholeWord", false);
        wrapAround = settings.GetValue("WrapAround", false);
      };

      Closing += (sender, args) => {
        settings.SetValue("Left", Left);
        settings.SetValue("Top", Top);
        settings.SetValue("Height", Height);
        settings.SetValue("Width", Width);
        
        settings.SetValue("MatchCase", matchCase);
        settings.SetValue("WholeWord", wholeWord);
        settings.SetValue("WrapAround", wrapAround);
      };

      Updater.Subscribe(
        (message, isError) => {
          if (InvokeRequired)
            Invoke(new Action(() => MessageBox.Show(message, Updater.ApplicationName, MessageBoxButtons.OK, isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information)));
          else
            MessageBox.Show(message, Updater.ApplicationName, MessageBoxButtons.OK, isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        },
        message => InvokeRequired
          ? (bool)Invoke(new Func<bool>(() => MessageBox.Show(this, message, Updater.ApplicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK))
          : MessageBox.Show(this, message, Updater.ApplicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK,
        () => { ExitToolStripMenuItem_Click(null, EventArgs.Empty); }
      );
      
      InitializeTheme();

      this.Load += (s, e) => {
        this.Width += 1;
      };
    }

    #region themes

    private void OnCurrentThemeChanged() {
      m_Lex.DefaultColor = Theme.Current.ForegroundColor;
      m_Lex.KeywordColor = Theme.Current.SelectedBackgroundColor;
      m_Lex.GreyKeywordColor = Theme.Current.InfoColor;
      m_Lex.FunctionColor = Theme.Current.HyperlinkColor;
      m_Lex.ValueColor = Theme.Current.WarnColor;
      m_Lex.CommentColor = Theme.Current.MessageColor;
    }

    private void InitializeTheme() {
      var currentItem = CustomTheme.FillThemesMenu<ToolStripRadioButtonMenuItem>((title, theme, onClick) => {
        if (theme == null && onClick == null) {
          themeMenuItem.DropDownItems.Add(new ToolStripSeparator());
          return null;
        }
        var item = new ToolStripRadioButtonMenuItem(title, null, onClick);
        themeMenuItem.DropDownItems.Add(item);
        return item;
      }, null, ThemeSupport.CurrentId, "SqlProfiler.themes");
      if (currentItem != null)
        currentItem.Checked = true;

      OnCurrentThemeChanged();
      ThemeSupport.Changed += OnCurrentThemeChanged;
      Closing += (_, _) => ThemeSupport.Changed -= OnCurrentThemeChanged;
    }

    protected override void OnHandleCreated(EventArgs e) {
      base.OnHandleCreated(e);
      ThemeSupport.Apply(this);
    }

    #endregion

    public sealed override Size MinimumSize {
      get { return base.MinimumSize; }
      set { base.MinimumSize = value; }
    }

    private void ParseCommandLine() {
      try {
        m_autostart = CommandLineParser.Apply(Environment.GetCommandLineArgs(), m_currentsettings, settings.SetValue);
        if (string.IsNullOrEmpty(settings.GetValue("Server", ""))) {
          settings.SetValue("Server", @".\sqlexpress");
        }
      }
      catch (Exception e) {
        MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void TbStart_Click(object sender, EventArgs e) {
      if (!TraceProperties.AtLeastOneEventSelected(m_currentsettings)) {
        MessageBox.Show("You should select at least 1 event", "Starting trace", MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        RunProfiling(true);
        return;
      }

      RunProfiling(false);
    }

    private void UpdateButtons() {
      tbStart.Enabled = m_ProfilingState == ProfilingStateEnum.psStopped ||
                        m_ProfilingState == ProfilingStateEnum.psPaused;
      tbRun.Enabled = tbStart.Enabled;
      mnRun.Enabled = tbRun.Enabled;
      tbRunWithFilters.Enabled = ProfilingStateEnum.psStopped == m_ProfilingState;
      mnRunWithFilters.Enabled = tbRunWithFilters.Enabled;
      startTraceToolStripMenuItem.Enabled = tbStart.Enabled;
      tbStop.Enabled = m_ProfilingState == ProfilingStateEnum.psPaused ||
                       m_ProfilingState == ProfilingStateEnum.psProfiling;
      stopTraceToolStripMenuItem.Enabled = tbStop.Enabled;
      tbPause.Enabled = m_ProfilingState == ProfilingStateEnum.psProfiling;
      pauseTraceToolStripMenuItem.Enabled = tbPause.Enabled;
      timer1.Enabled = m_ProfilingState == ProfilingStateEnum.psProfiling;
      edServer.Enabled = m_ProfilingState == ProfilingStateEnum.psStopped;
      tbAuth.Enabled = m_ProfilingState == ProfilingStateEnum.psStopped;
      edUser.Enabled = edServer.Enabled && (tbAuth.SelectedIndex == 1);
      edPassword.Enabled = edServer.Enabled && (tbAuth.SelectedIndex == 1);
    }

    private void InitLV() {
      lvEvents = new ListViewNF {
        Dock = DockStyle.Fill,
        Location = new System.Drawing.Point(0, 0),
        Name = "lvEvents",
        Size = new System.Drawing.Size(979, 297),
        TabIndex = 0,
        VirtualMode = true,
        UseCompatibleStateImageBehavior = false,
        BorderStyle = BorderStyle.None,
        FullRowSelect = true,
        View = View.Details,
        GridLines = true,
        HideSelection = false,
        MultiSelect = true,
        AllowColumnReorder = false
      };
      lvEvents.RetrieveVirtualItem += LvEvents_RetrieveVirtualItem;
      lvEvents.ItemSelectionChanged += LvEvents_ItemSelectionChanged;
      lvEvents.ColumnClick += LvEvents_ColumnClick;
      lvEvents.SelectedIndexChanged += LvEvents_SelectedIndexChanged;
      lvEvents.VirtualItemsSelectionRangeChanged += LvEventsOnVirtualItemsSelectionRangeChanged;
      lvEvents.ContextMenuStrip = contextMenu;
      splitContainer1.Panel1.Controls.Add(lvEvents);
      InitColumns();
      InitGridColumns();
    }

    private void InitColumns() {
      m_columns.Clear();
      m_columns.Add(new PerfColumn {Caption = "Event Class", Column = ProfilerEventColumns.EventClass, Width = 122});
      fixedColumnIndex = m_columns.Count;
      m_columns.Add(new PerfColumn {Caption = "Text Data", Column = ProfilerEventColumns.TextData, Width = 255});
      m_columns.Add(new PerfColumn {Caption = "Login Name", Column = ProfilerEventColumns.LoginName, Width = 79});
      m_columns.Add(new PerfColumn {
        Caption = "CPU", Column = ProfilerEventColumns.CPU, Width = 82, Alignment = HorizontalAlignment.Right,
        Format = "#,0"
      });
      m_columns.Add(new PerfColumn {
        Caption = "Reads", Column = ProfilerEventColumns.Reads, Width = 78, Alignment = HorizontalAlignment.Right,
        Format = "#,0"
      });
      m_columns.Add(new PerfColumn {
        Caption = "Writes", Column = ProfilerEventColumns.Writes, Width = 78, Alignment = HorizontalAlignment.Right,
        Format = "#,0"
      });
      m_columns.Add(new PerfColumn {
        Caption = "Duration, ms", Column = ProfilerEventColumns.Duration, Width = 82,
        Alignment = HorizontalAlignment.Right, Format = "#,0"
      });
      m_columns.Add(new PerfColumn
        {Caption = "SPID", Column = ProfilerEventColumns.SPID, Width = 50, Alignment = HorizontalAlignment.Right});

      if (m_currentsettings.EventsColumns.StartTime)
        m_columns.Add(new PerfColumn {
          Caption = "Start time", Column = ProfilerEventColumns.StartTime, Width = 140,
          Format = "yyyy-MM-dd HH:mm:ss.ffff"
        });
      if (m_currentsettings.EventsColumns.EndTime)
        m_columns.Add(new PerfColumn {
          Caption = "End time", Column = ProfilerEventColumns.EndTime, Width = 140, Format = "yyyy-MM-dd HH:mm:ss.ffff"
        });
      if (m_currentsettings.EventsColumns.DatabaseName)
        m_columns.Add(new PerfColumn
          {Caption = "DatabaseName", Column = ProfilerEventColumns.DatabaseName, Width = 70});
      if (m_currentsettings.EventsColumns.ObjectName)
        m_columns.Add(new PerfColumn {Caption = "Object name", Column = ProfilerEventColumns.ObjectName, Width = 70});
      if (m_currentsettings.EventsColumns.ApplicationName)
        m_columns.Add(new PerfColumn
          {Caption = "Application name", Column = ProfilerEventColumns.ApplicationName, Width = 70});
      if (m_currentsettings.EventsColumns.HostName)
        m_columns.Add(new PerfColumn {Caption = "Host name", Column = ProfilerEventColumns.HostName, Width = 70});

      m_columns.Add(new PerfColumn {Caption = "#", Column = -1, Width = 53, Alignment = HorizontalAlignment.Right});
    }

    private void InitGridColumns() {
      InitColumns();
      lvEvents.BeginUpdate();
      lvEvents.SizeChanged -= LvEvents_SizeChanged;
      lvEvents.ColumnWidthChanged -= LvEvents_ColumnWidthChanged;
      try {
        lvEvents.Columns.Clear();
        foreach (var pc in m_columns) {
          var l = lvEvents.Columns.Add(pc.Caption, pc.Width);
          l.TextAlign = pc.Alignment;
        }
      }
      finally {
        lvEvents.EndUpdate();
        lvEvents.SizeChanged += LvEvents_SizeChanged;
        lvEvents.ColumnWidthChanged += LvEvents_ColumnWidthChanged;
      }
    }

    private void LvEvents_SizeChanged(object sender, EventArgs e) {
      var otherColumnsWidth = 0;
      for (var i = 0; i < lvEvents.Columns.Count; i++) {
        if (i == fixedColumnIndex) continue;
        otherColumnsWidth += lvEvents.Columns[i].Width;
      }
      
      var width = lvEvents.Width - otherColumnsWidth - SystemInformation.VerticalScrollBarWidth;
      lvEvents.Columns[fixedColumnIndex].Width = Math.Max(width, MinFixedColumnWidth);
    }

    private void LvEvents_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e) {
      if (e.ColumnIndex == fixedColumnIndex)
        return;
      LvEvents_SizeChanged(sender, EventArgs.Empty);
    }

    private void LvEventsOnVirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e) {
      if (!e.IsSelected) return;
      UpdateSourceBox();
    }

    void LvEvents_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateSourceBox();
    }

    void LvEvents_ColumnClick(object sender, ColumnClickEventArgs e) {
      lvEvents.ToggleSortOrder();
      lvEvents.SetSortIcon(e.Column, lvEvents.SortOrder);
      var comparer = new TextDataComparer(e.Column, lvEvents.SortOrder);
      m_Cached.Sort(comparer);
      UpdateSourceBox();
      ShowSelectedEvent();
    }

    private string GetEventCaption(ProfilerEvent evt) {
      if (evt == m_EventStarted) {
        return "Trace started";
      }

      if (evt == m_EventPaused) {
        return "Trace paused";
      }

      if (evt == m_EventStopped) {
        return "Trace stopped";
      }

      var eventClass = evt.EventClass;
      return eventClass >= 0 && eventClass < ProfilerEvents.Names.Length && ProfilerEvents.Names[eventClass] != ""
        ? ProfilerEvents.Names[eventClass]
        : "Event " + eventClass;
    }

    private string GetFormattedValue(ProfilerEvent evt, int column, string format) {
      return ProfilerEventColumns.Duration == column
        ? (evt.Duration / 1000).ToString(format)
        : evt.GetFormattedData(column, format);
    }

    private void NewEventArrived(ProfilerEvent evt, bool last) {
      {
        var current = (lvEvents.SelectedIndices.Count > 0) ? m_Cached[lvEvents.SelectedIndices[0]] : null;
        m_EventCount++;
        var caption = GetEventCaption(evt);
        var lvi = new ListViewItem(caption);
        var items = new string[m_columns.Count];
        for (var i = 1; i < m_columns.Count; i++) {
          var pc = m_columns[i];
          items[i - 1] = pc.Column == -1
            ? m_EventCount.ToString("#,0")
            : GetFormattedValue(evt, pc.Column, pc.Format) ?? "";
        }

        lvi.SubItems.AddRange(items);
        lvi.Tag = evt;
        m_Cached.Add(lvi);
        if (last) {
          lvEvents.VirtualListSize = m_Cached.Count;
          if (autoScrollToolStripMenuItem.Checked) {
            lvEvents.SelectedIndices.Clear();
            FocusLVI(lvEvents.Items[m_Cached.Count - 1], autoScrollToolStripMenuItem.Checked);
          }
          //FocusLVI(tbScroll.Checked ? lvEvents.Items[m_Cached.Count - 1] : current, tbScroll.Checked);
          lvEvents.Invalidate(lvi.Bounds);
        }
      }
    }

    internal void FocusLVI(ListViewItem lvi, bool ensure) {
      if (lvi == null) return;
      lvi.Focused = true;
      lvi.Selected = true;
      LvEvents_ItemSelectionChanged(lvEvents, null);
      if (ensure) {
        lvEvents.EnsureVisible(lvEvents.Items.IndexOf(lvi));
      }
    }

    private void ReadEvents(CancellationToken token) {
      try {
        while (!token.IsCancellationRequested && m_Rdr.TraceIsActive) {
          var evt = m_Rdr.Next();
          if (evt != null) {
            lock (m_eventsLock) {
              m_events.Enqueue(evt);
            }
          }
        }
      }
      catch (Exception e) {
        lock (m_eventsLock) {
          if (!token.IsCancellationRequested && m_Rdr.TraceIsActive) {
            m_profilerexception = e;
          }
        }
      }
    }

    private SqlConnection GetConnection() {
      var builder = new SqlConnectionStringBuilder {
        DataSource = edServer.Text,
        InitialCatalog = "master",
        ApplicationName = "SqlProfiler",
        ConnectTimeout = 15
      };
      if (tbAuth.SelectedIndex == 0) {
        builder.IntegratedSecurity = true;
      }
      else {
        builder.UserID = edUser.Text;
        builder.Password = edPassword.Text;
      }

      return new SqlConnection(builder.ConnectionString);
    }

    private void StartProfiling() {
      try {
        Cursor = Cursors.WaitCursor;
        m_perf.Clear();
        m_first = null;
        m_prev = null;
        if (m_ProfilingState == ProfilingStateEnum.psPaused) {
          ResumeProfiling();
          return;
        }

        if (m_Conn != null && m_Conn.State == ConnectionState.Open) {
          m_Conn.Close();
        }

        InitGridColumns();
        m_EventCount = 0;
        m_Conn = GetConnection();
        m_Conn.Open();
        m_Rdr = new RawTraceReader(m_Conn);

        m_Rdr.CreateTrace();
        TraceConfigurator.Apply(m_Rdr, m_currentsettings);
        m_Cached.Clear();
        m_events.Clear();
        lvEvents.VirtualListSize = 0;
        StartProfilerThread();
      }
      catch (SqlException e) when (IsPermissionError(e)) {
        MessageBox.Show(e.Message + "\r\n\r\nThe login needs the ALTER TRACE permission on the server (or the sysadmin role).",
          "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch (Exception e) {
        MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        UpdateButtons();
        Cursor = Cursors.Default;
      }
    }

    private static bool IsPermissionError(SqlException e) {
      foreach (SqlError error in e.Errors) {
        // 229: permission denied, 8189: no permission to run the trace procedure, 15247: no permission to perform the action
        if (error.Number == 229 || error.Number == 8189 || error.Number == 15247) return true;
      }

      return false;
    }

    private void StopReaderTask() {
      m_readerCts?.Cancel();
      m_Rdr.Close();
      // the task ends when the closed reader stops its blocking read; a stuck read must not freeze the window
      m_readerTask?.Wait(3000);
      m_readerTask = null;
      m_readerCts?.Dispose();
      m_readerCts = null;
    }

    private void StartProfilerThread() {
      StopReaderTask();
      m_Rdr.StartTrace();
      m_readerCts = new CancellationTokenSource();
      var token = m_readerCts.Token;
      m_ProfilingState = ProfilingStateEnum.psProfiling;
      NewEventArrived(m_EventStarted, true);
      m_readerTask = Task.Factory.StartNew(() => ReadEvents(token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private void ResumeProfiling() {
      StartProfilerThread();
      UpdateButtons();
    }

    private void TbStop_Click(object sender, EventArgs e) {
      StopProfiling();
    }

    private void StopProfiling() {
      tbStop.Enabled = false;
      using (var cn = GetConnection()) {
        cn.Open();
        m_Rdr.StopTrace(cn);
        m_Rdr.CloseTrace(cn);
      }

      StopReaderTask();
      m_ProfilingState = ProfilingStateEnum.psStopped;
      NewEventArrived(m_EventStopped, true);
      UpdateButtons();
    }

    private void LvEvents_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
      UpdateSourceBox();
    }

    private int[] prevSelectedIndices = null;

    private void UpdateSourceBox() {
      if (dontUpdateSource) return;
      var sb = new StringBuilder();

      int[] selectedIndices = new int[lvEvents.SelectedIndices.Count];
      lvEvents.SelectedIndices.CopyTo(selectedIndices, 0);
      if (prevSelectedIndices != null && Enumerable.SequenceEqual(selectedIndices, prevSelectedIndices))
        return;
      prevSelectedIndices = selectedIndices;

      foreach (int i in selectedIndices) {
        var lv = m_Cached[i];
        if (lv.SubItems[1].Text != "") {
          sb.AppendFormat("{0}\r\ngo\r\n", lv.SubItems[1].Text);
        }
      }

      m_Lex.FillRichEdit(reTextData, sb.ToString());
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      if (m_ProfilingState == ProfilingStateEnum.psPaused || m_ProfilingState == ProfilingStateEnum.psProfiling) {
        if (MessageBox.Show("There are traces still running. Are you sure you want to close the application?",
              "SqlProfiler", MessageBoxButtons.YesNo, MessageBoxIcon.Question
              , MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
          StopProfiling();
        }
        else {
          e.Cancel = true;
        }
      }
    }

    private void LvEvents_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
      e.Item = m_Cached[e.ItemIndex];
    }

    private void TbPause_Click(object sender, EventArgs e) {
      PauseProfiling();
    }

    private void PauseProfiling() {
      using (var cn = GetConnection()) {
        cn.Open();
        m_Rdr.StopTrace(cn);
      }

      m_ProfilingState = ProfilingStateEnum.psPaused;
      NewEventArrived(m_EventPaused, true);
      UpdateButtons();
    }

    internal void SelectAllEvents(bool select) {
      lock (m_Cached) {
        lvEvents.BeginUpdate();
        dontUpdateSource = true;
        try {
          foreach (var lv in m_Cached) {
            lv.Selected = select;
          }
        }
        finally {
          dontUpdateSource = false;
          UpdateSourceBox();
          lvEvents.EndUpdate();
        }
      }
    }

    private void Timer1_Tick(object sender, EventArgs e) {
      Queue<ProfilerEvent> saved;
      Exception exc;
      lock (m_eventsLock) {
        saved = m_events;
        m_events = new Queue<ProfilerEvent>(10);
        exc = m_profilerexception;
        m_profilerexception = null;
      }

      if (null != exc) {
        using var dlg = new ThreadExceptionDialog(exc);
        dlg.ShowDialog();
      }

      lock (m_Cached) {
        while (0 != saved.Count) {
          NewEventArrived(saved.Dequeue(), 0 == saved.Count);
        }

        if (m_Cached.Count > m_currentsettings.Filters.MaximumEventCount) {
          m_Cached.RemoveRange(0, m_Cached.Count - Math.Max(m_currentsettings.Filters.MaximumEventCount, 0));
          lvEvents.VirtualListSize = m_Cached.Count;
          lvEvents.Invalidate();
        }

        if ((null == m_prev) || (DateTime.Now.Subtract(m_prev.m_date).TotalSeconds >= 1)) {
          var curr = new PerfInfo {m_count = m_EventCount};
          if (m_perf.Count >= 60) {
            m_first = m_perf.Dequeue();
          }

          if (null == m_first) m_first = curr;
          if (null == m_prev) m_prev = curr;

          var now = DateTime.Now;
          var d1 = now.Subtract(m_prev.m_date).TotalSeconds;
          var d2 = now.Subtract(m_first.m_date).TotalSeconds;
          slEPS.Text = string.Format("{0} / {1} EPS(last/avg for {2} second(s))",
            (Math.Abs(d1 - 0) > 0.001 ? ((curr.m_count - m_prev.m_count) / d1).ToString("#,0.00") : ""),
            (Math.Abs(d2 - 0) > 0.001 ? ((curr.m_count - m_first.m_count) / d2).ToString("#,0.00") : ""),
            d2.ToString("0"));

          m_perf.Enqueue(curr);
          m_prev = curr;
        }
      }
    }

    private void TbAuth_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateButtons();
    }

    private void ClearTrace() {
      lock (m_Cached) {
        m_Cached.Clear();
        lvEvents.VirtualListSize = 0;
        LvEvents_ItemSelectionChanged(lvEvents, null);
        lvEvents.Invalidate();
      }
    }

    private void TbClear_Click(object sender, EventArgs e) {
      ClearTrace();
    }

    private void CopyAllToClipboardToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(false);
    }

    private void CopyEventsToClipboard(bool copySelected) {
      string xml;
      lock (m_Cached) {
        var items = copySelected
          ? lvEvents.SelectedIndices.Cast<int>().Select(i => m_Cached[i])
          : m_Cached;
        xml = EventExporter.BuildEventsXml(items.Select(i => (ProfilerEvent) i.Tag).ToList());
      }

      Clipboard.SetText(xml);
      MessageBox.Show("Event(s) data copied to clipboard", "Information", MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }

    private void CopySelectedToClipboardToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(true);
    }

    private void ClearTraceWindowToolStripMenuItem_Click(object sender, EventArgs e) {
      ClearTrace();
    }

    private void ExtractAllEventsToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(false);
    }

    private void ExtractSelectedEventsToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(true);
    }

    private void PauseTraceToolStripMenuItem_Click(object sender, EventArgs e) {
      PauseProfiling();
    }

    private void StopTraceToolStripMenuItem_Click(object sender, EventArgs e) {
      StopProfiling();
    }

    private void FindToolStripMenuItem_Click(object sender, EventArgs e) {
      DoFind();
    }

    private void DoFind() {
      if (m_ProfilingState == ProfilingStateEnum.psProfiling) {
        MessageBox.Show("You cannot find when trace is running", "SqlProfiler", MessageBoxButtons.OK,
          MessageBoxIcon.Information);
        return;
      }

      using (var f = new FindForm(this)) {
        f.TopMost = this.TopMost;
        f.ShowDialog();
      }
    }

    private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e) {
      if (lvEvents.Focused && (m_ProfilingState != ProfilingStateEnum.psProfiling)) {
        SelectAllEvents(true);
      }
      else if (reTextData.Focused) {
        reTextData.SelectAll();
      }
    }

    internal void PerformFind(bool forwards, bool wrapAround) {
      if (string.IsNullOrEmpty(lastpattern)) return;
      var lastpos = lvEvents.Items.IndexOf(lvEvents.FocusedItem);
      if (forwards) {
        for (var i = lastpos + 1; i < m_Cached.Count; i++) {
          if (FindText(i)) {
            return;
          }
        }

        if (wrapAround) {
          for (var i = 0; i < lastpos; i++) {
            if (FindText(i)) {
              return;
            }
          }
        }
      }
      else {
        for (var i = lastpos - 1; i >= 0; i--) {
          if (FindText(i)) {
            return;
          }
        }

        if (wrapAround) {
          for (var i = m_Cached.Count - 1; i > lastpos; i--) {
            if (FindText(i)) {
              return;
            }
          }
        }
      }

      MessageBox.Show(string.Format("Failed to find \"{0}\". Searched to the end of data. ", lastpattern),
        "SqlProfiler", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowSelectedEvent() {
      var focusedIndex = lvEvents.Items.IndexOf(lvEvents.FocusedItem);
      if ((focusedIndex > -1) && (focusedIndex < m_Cached.Count)) {
        var lvi = m_Cached[focusedIndex];
        var evt = (ProfilerEvent) lvi.Tag;

        lvi.Focused = true;
        SelectAllEvents(false);
        FocusLVI(lvi, true);
      }
    }

    private bool FindText(int i) {
      var lvi = m_Cached[i];
      var evt = (ProfilerEvent) lvi.Tag;
      var escaped = Regex.Escape(lastpattern);
      var pattern = wholeWord ? "\\b" + escaped + "\\b" : escaped;
      if (Regex.IsMatch(evt.TextData, pattern, (matchCase ? RegexOptions.None : RegexOptions.IgnoreCase))) {
        lvi.Focused = true;
        SelectAllEvents(false);
        FocusLVI(lvi, true);
        return true;
      }

      return false;
    }

    private void FindNextToolStripMenuItem_Click(object sender, EventArgs e) {
      if (m_ProfilingState == ProfilingStateEnum.psProfiling) {
        MessageBox.Show("You cannot find when trace is running", "SqlProfiler", MessageBoxButtons.OK,
          MessageBoxIcon.Information);
        return;
      }

      PerformFind(true, wrapAround);
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    internal void RunProfiling(bool showfilters) {
      if (showfilters) {
        var ts = m_currentsettings.GetCopy();
        using (var frm = new TraceProperties()) {
          frm.SetSettings(ts);
          if (DialogResult.OK != frm.ShowDialog()) return;
          m_currentsettings = frm.m_currentsettings.GetCopy();
        }
      }

      StartProfiling();
    }

    private void TbRunWithFilters_Click(object sender, EventArgs e) {
      RunProfiling(true);
    }

    private void CopyToXlsToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyForExcel();
    }

    private XmlDocument BuildExcelDocument() {
      lock (m_Cached) {
        return EventExporter.BuildExcelDocument(lvEvents, m_columns, m_Cached.ToList(), GetEventCaption, GetFormattedValue);
      }
    }

    private void CopyForExcel() {
      var doc = BuildExcelDocument();
      using (var xmlStream = new MemoryStream(EventExporter.GetXmlBytes(doc))) {
        Clipboard.SetData("XML Spreadsheet", xmlStream);
      }

      MessageBox.Show("Event(s) data copied to clipboard", "Information", MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }

    private void MnAbout_Click(object sender, EventArgs e) {
      Updater.ShowAbout();
    }

    private void StayOnTop_Click(object sender, EventArgs e) {
      stayOnTopToolStripMenuItem.Checked = !stayOnTopToolStripMenuItem.Checked;
      this.TopMost = stayOnTopToolStripMenuItem.Checked;
    }

    protected override void OnMouseWheel(MouseEventArgs e) {
      if (e.Button != MouseButtons.None) return;
      var delta = e.Delta > 0 ? 0.05 : -0.05;
      var nextOpacity = Opacity + delta;
      if (nextOpacity < 0.1) nextOpacity = 0.1;
      if (nextOpacity > 1) nextOpacity = 1;
      Opacity = nextOpacity;
    }

    private void DeleteSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
      lock (m_Cached) {
        for (var i = lvEvents.SelectedIndices.Count - 1; i >= 0; i--) {
          m_Cached.RemoveAt(lvEvents.SelectedIndices[i]);
        }
      }

      lvEvents.VirtualListSize = m_Cached.Count;
      lvEvents.SelectedIndices.Clear();
    }

    private void KeepSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
      lock (m_Cached) {
        var selected = new HashSet<int>(lvEvents.SelectedIndices.Cast<int>());
        for (var i = m_Cached.Count - 1; i >= 0; i--) {
          if (!selected.Contains(i)) {
            m_Cached.RemoveAt(i);
          }
        }
      }

      lvEvents.VirtualListSize = m_Cached.Count;
      lvEvents.SelectedIndices.Clear();
    }

    private void SaveToExcelXmlFile() {
      var doc = BuildExcelDocument();

      using (var sfd = new SaveFileDialog {Filter = "Excel XML|*.xml", Title = "Save the Excel XML file"}) {
        if (sfd.ShowDialog(this) != DialogResult.OK || string.IsNullOrEmpty(sfd.FileName)) return;
        File.WriteAllBytes(sfd.FileName, EventExporter.GetXmlBytes(doc));

        MessageBox.Show(string.Format("File saved to: {0}", sfd.FileName), "Information", MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }

    private void SetFilterEvents() {
      if (m_CachedUnFiltered.Count == 0) {
        lvEvents.SelectedIndices.Clear();
        using (var frm = new TraceProperties()) {
          frm.SetSettings(m_currentsettings.GetCopy());
          if (DialogResult.OK != frm.ShowDialog()) return;
          m_currentsettings = frm.m_currentsettings.GetCopy();

          m_CachedUnFiltered.AddRange(m_Cached);
          m_Cached.Clear();
          foreach (var lvi in m_CachedUnFiltered) {
            if (frm.IsIncluded(lvi) && m_Cached.Count < m_currentsettings.Filters.MaximumEventCount) {
              m_Cached.Add(lvi);
            }
          }
        }

        lvEvents.VirtualListSize = m_Cached.Count;
        UpdateSourceBox();
        ShowSelectedEvent();
      }
    }

    private void ClearFilterEvents() {
      if (m_CachedUnFiltered.Count > 0) {
        m_Cached.Clear();
        m_Cached.AddRange(m_CachedUnFiltered);
        m_CachedUnFiltered.Clear();
        lvEvents.VirtualListSize = m_Cached.Count;
        lvEvents.SelectedIndices.Clear();
        UpdateSourceBox();
        ShowSelectedEvent();
      }
    }

    private void SaveAllEventsToExcelXmlFileToolStripMenuItem_Click(object sender, EventArgs e) {
      SaveToExcelXmlFile();
    }

    private void EdServer_TextChanged(object sender, EventArgs e) {
      settings.SetValue("Server", edServer.Text);
    }

    private void EdUser_TextChanged(object sender, EventArgs e) {
      settings.SetValue("User", edUser.Text);
    }

    private void EdPassword_TextChanged(object sender, EventArgs e) {
      settings.SetValue("Password", edPassword.Text);
    }

    private void FilterCapturedEventsToolStripMenuItem_Click(object sender, EventArgs e) {
      SetFilterEvents();
    }

    private void ClearCapturedFiltersToolStripMenuItem_Click(object sender, EventArgs e) {
      ClearFilterEvents();
    }

    private void SiteToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.VisitAppSite();
    }

    private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.CheckForUpdates(Updater.CheckUpdatesMode.AllMessages);
    }

    private void TbFilterEvents_Click(object sender, EventArgs e) {
      var filterButton = (ToolStripButton) sender;
      if (filterButton.Checked) {
        SetFilterEvents();
      }
      else {
        ClearFilterEvents();
      }
    }
  }
}