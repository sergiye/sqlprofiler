using sergiye.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using SortOrder = System.Windows.Forms.SortOrder;

namespace SqlProfiler {

  public partial class MainForm : Form {

    private const int MinFixedColumnWidth = 150;

    private readonly PersistentSettings _settings;
    private readonly YukonLexer _lexer = new();
    private readonly TraceSession _session;
    private readonly EventRateMeter _rateMeter = new();
    private readonly List<EventRow> _rows = new(1024);
    private readonly List<EventRow> _unfilteredRows = new(1024);
    private TraceProperties.TraceFilters _activeFilters;
    private readonly QuickFilter _quickFilter = new();
    private bool _filtering;
    private readonly ProfilerEvent _startedEvent = new();
    private readonly ProfilerEvent _stoppedEvent = new();
    private readonly ProfilerEvent _pausedEvent = new();
    private List<PerfColumn> _columns = new();
    private ListViewNF lvEvents;
    private TraceProperties.TraceSettings _traceSettings;
    private int _eventCount;
    private bool _autoStart;
    private bool _dontUpdateSource;
    private int[] _prevSelectedIndices;
    private UserOption _encrypt;
    private UserOption _trustCertificate;

    internal string LastPattern { get; set; } = "";
    internal bool MatchCase { get; set; }
    internal bool WholeWord { get; set; }

    public MainForm() {
      _settings = new PersistentSettings();
      _settings.Load();
      ThemeSupport.Initialize(_settings.GetValue("Theme", ""), id => _settings.SetValue("Theme", id), "SqlProfiler.themes");

      InitializeComponent();
      _session = new TraceSession(CreateConnection);
      tbStart.DefaultItem = tbRun;
      Icon = Icon.ExtractAssociatedIcon(Updater.CurrentFileLocation);
      Text = $"{Updater.ApplicationName} {(Environment.Is64BitProcess ? "x64" : "x32")} - {Updater.CurrentVersion}";
      StartPosition = FormStartPosition.CenterScreen;
      edPassword.TextBox.PasswordChar = '*';

      ThemedWindowTitle.Attach(this, mainMenu, WindowTitleStyle.ThemeColors);
      InitShortcuts();
      InitStatusBar();
      InitCopyText();
      InitRowMenu();
      InitHighlight();
      InitSourcePanel();
      InitQuickSearch();
      InitConnectionTools();
      InitPresets();
      InitTraceFile();

      _traceSettings = LoadTraceSettings();
      ParseCommandLine();
      InitLV();
      RestoreSortState();

      var portable = new UserOption("portable", true, storeSettingsInFileToolStripMenuItem, _settings);
      portable.Changed += delegate {
        _settings.IsPortable = portable.Value;
      };

      _ = new UserOption("autoScroll", true, autoScrollToolStripMenuItem, _settings);

      var encryptItem = new ToolStripMenuItem("Encrypt connection");
      var trustCertificateItem = new ToolStripMenuItem("Trust server certificate");
      var insertAt = fileToolStripMenuItem.DropDownItems.IndexOf(storeSettingsInFileToolStripMenuItem) + 1;
      fileToolStripMenuItem.DropDownItems.Insert(insertAt, encryptItem);
      fileToolStripMenuItem.DropDownItems.Insert(insertAt + 1, trustCertificateItem);
      _encrypt = new UserOption("encrypt", false, encryptItem, _settings);
      _trustCertificate = new UserOption("trustServerCertificate", true, trustCertificateItem, _settings);

      var openLogItem = new ToolStripMenuItem("Open log file", null, (_, _) => OpenLogFile());
      helpToolStripMenuItem.DropDownItems.Insert(helpToolStripMenuItem.DropDownItems.IndexOf(aboutToolStripMenuItem), openLogItem);

      edServer.Text = _settings.GetValue("Server", ".");
      edUser.Text = _settings.GetValue("User", "");
      edPassword.Text = _settings.GetValue("Password", "");
      tbAuth.SelectedIndex = string.IsNullOrEmpty(edUser.Text) ? 0 : 1;
      if (_autoStart) RunProfiling(false);
      UpdateButtons();

      MinimumSize = new Size(1100, 666);

      Load += (_, _) => {
        RestoreLayout();

        MatchCase = _settings.GetValue("MatchCase", false);
        WholeWord = _settings.GetValue("WholeWord", false);
        _tbMatchCase.Checked = MatchCase;
        _tbWholeWord.Checked = WholeWord;
      };

      Closing += (_, _) => {
        SaveLayout();
        SaveColumnWidths();
        SaveTraceSettings();

        _settings.SetValue("SortColumn", lvEvents.SortColumn);
        _settings.SetValue("SortOrder", (int) lvEvents.SortOrder);
        _settings.SetValue("MatchCase", MatchCase);
        _settings.SetValue("WholeWord", WholeWord);
      };

      FormClosed += (_, _) => _session.Dispose();

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

      Load += (_, _) => {
        Width += 1;
      };
    }

    #region themes

    private void OnCurrentThemeChanged() {
      _lexer.DefaultColor = Theme.Current.ForegroundColor;
      _lexer.KeywordColor = Theme.Current.SelectedBackgroundColor;
      _lexer.GreyKeywordColor = Theme.Current.InfoColor;
      _lexer.FunctionColor = Theme.Current.HyperlinkColor;
      _lexer.ValueColor = Theme.Current.WarnColor;
      _lexer.CommentColor = Theme.Current.MessageColor;
      lvEvents?.Invalidate();
    }

    private void InitializeTheme() {
      var currentItem = CustomTheme.FillThemesMenu<ToolStripRadioButtonMenuItem>((title, theme, onClick) => {
        if (theme is null && onClick is null) {
          themeMenuItem.DropDownItems.Add(new ToolStripSeparator());
          return null;
        }
        var item = new ToolStripRadioButtonMenuItem(title, null, onClick);
        themeMenuItem.DropDownItems.Add(item);
        return item;
      }, null, ThemeSupport.CurrentId, "SqlProfiler.themes");
      if (currentItem is not null)
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
        _autoStart = CommandLineParser.Apply(Environment.GetCommandLineArgs(), _traceSettings, _settings.SetValue, _presets.Load);
        if (string.IsNullOrEmpty(_settings.GetValue("Server", ""))) {
          _settings.SetValue("Server", @".\sqlexpress");
        }
      }
      catch (Exception e) {
        MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private SqlConnection CreateConnection() {
      return ConnectionFactory.Create(new ConnectionOptions {
        Server = edServer.Text,
        IntegratedSecurity = tbAuth.SelectedIndex == 0,
        User = edUser.Text,
        Password = edPassword.Text,
        Encrypt = _encrypt.Value,
        TrustServerCertificate = _trustCertificate.Value
      });
    }

    private void OpenLogFile() {
      if (!File.Exists(AppLogger.FilePath)) {
        MessageBox.Show("The log is empty.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      Process.Start(new ProcessStartInfo(AppLogger.FilePath) {UseShellExecute = true});
    }

    private void TbStart_Click(object sender, EventArgs e) {
      if (!TraceProperties.AtLeastOneEventSelected(_traceSettings)) {
        MessageBox.Show("You should select at least 1 event", "Starting trace", MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        RunProfiling(true);
        return;
      }

      RunProfiling(false);
    }

    private void UpdateButtons() {
      var state = _session.State;
      tbStart.Enabled = state == ProfilingStateEnum.psStopped || state == ProfilingStateEnum.psPaused;
      tbRun.Enabled = tbStart.Enabled;
      mnRun.Enabled = tbRun.Enabled;
      tbRunWithFilters.Enabled = state == ProfilingStateEnum.psStopped;
      mnRunWithFilters.Enabled = tbRunWithFilters.Enabled;
      startTraceToolStripMenuItem.Enabled = tbStart.Enabled;
      tbStop.Enabled = state == ProfilingStateEnum.psPaused || state == ProfilingStateEnum.psProfiling;
      stopTraceToolStripMenuItem.Enabled = tbStop.Enabled;
      tbPause.Enabled = state == ProfilingStateEnum.psProfiling;
      pauseTraceToolStripMenuItem.Enabled = tbPause.Enabled;
      timer1.Enabled = state == ProfilingStateEnum.psProfiling;
      UpdateStatus();
      edServer.Enabled = state == ProfilingStateEnum.psStopped;
      tbAuth.Enabled = state == ProfilingStateEnum.psStopped;
      edUser.Enabled = edServer.Enabled && tbAuth.SelectedIndex == 1;
      edPassword.Enabled = edServer.Enabled && tbAuth.SelectedIndex == 1;
    }

    #region list

    private void InitLV() {
      lvEvents = new ListViewNF {
        Dock = DockStyle.Fill,
        Location = new Point(0, 0),
        Name = "lvEvents",
        Size = new Size(979, 297),
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
      InitGridColumns();
    }

    private void InitGridColumns() {
      _columns = GridColumns.Build(_traceSettings.EventsColumns);
      RestoreColumnWidths(_columns);
      lvEvents.BeginUpdate();
      lvEvents.SizeChanged -= LvEvents_SizeChanged;
      lvEvents.ColumnWidthChanged -= LvEvents_ColumnWidthChanged;
      try {
        lvEvents.Columns.Clear();
        foreach (var pc in _columns) {
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
        if (i == GridColumns.TextDataIndex) continue;
        otherColumnsWidth += lvEvents.Columns[i].Width;
      }

      var width = lvEvents.Width - otherColumnsWidth - SystemInformation.VerticalScrollBarWidth;
      lvEvents.Columns[GridColumns.TextDataIndex].Width = Math.Max(width, MinFixedColumnWidth);
    }

    private void LvEvents_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e) {
      if (e.ColumnIndex == GridColumns.TextDataIndex)
        return;
      LvEvents_SizeChanged(sender, EventArgs.Empty);
    }

    private void LvEventsOnVirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e) {
      if (!e.IsSelected) return;
      UpdateSourceBox();
    }

    private void LvEvents_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateSourceBox();
    }

    private void LvEvents_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
      UpdateSourceBox();
    }

    private void LvEvents_ColumnClick(object sender, ColumnClickEventArgs e) {
      lvEvents.ToggleSort(e.Column);
      lvEvents.SetSortIcon(e.Column, lvEvents.SortOrder);
      ApplySort();
      ShowSelectedEvent();
    }

    /// <summary>Sorts the rows by the column and direction shown in the header.</summary>
    private void ApplySort() {
      if (lvEvents.SortColumn < 0 || lvEvents.SortColumn >= _columns.Count || lvEvents.SortOrder == SortOrder.None) return;
      RowSorter.Sort(_rows, lvEvents.SortColumn, lvEvents.SortOrder, GetCellText);
      _prevSelectedIndices = null;
      lvEvents.Invalidate();
      UpdateSourceBox();
    }

    private void RestoreSortState() {
      var column = _settings.GetValue("SortColumn", -1);
      if (column < 0 || column >= _columns.Count) return;
      lvEvents.SortOrder = _settings.GetValue("SortOrder", (int) SortOrder.Ascending) == (int) SortOrder.Descending
        ? SortOrder.Descending
        : SortOrder.Ascending;
      lvEvents.SortColumn = column;
    }

    private void LvEvents_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
      e.Item = e.ItemIndex < _rows.Count ? CreateItem(_rows[e.ItemIndex]) : new ListViewItem();
    }

    private ListViewItem CreateItem(EventRow row) {
      var item = new ListViewItem(row.Caption) {Tag = row};
      if (GetRowBackColor(row) is { } back)
        item.BackColor = back;
      var cells = new string[_columns.Count - 1];
      for (var i = 1; i < _columns.Count; i++)
        cells[i - 1] = GetCellText(row, i);
      item.SubItems.AddRange(cells);
      return item;
    }

    private string GetCellText(EventRow row, int columnIndex) {
      if (columnIndex == 0) return row.Caption;
      var pc = _columns[columnIndex];
      return pc.Column == -1 ? row.Number.ToString("#,0") : GetFormattedValue(row.Event, pc.Column, pc.Format) ?? "";
    }

    private string GetEventCaption(ProfilerEvent evt) {
      if (evt == _startedEvent) return "Trace started";
      if (evt == _pausedEvent) return "Trace paused";
      if (evt == _stoppedEvent) return "Trace stopped";

      var eventClass = evt.EventClass;
      return eventClass >= 0 && eventClass < ProfilerEvents.Names.Length && ProfilerEvents.Names[eventClass] != ""
        ? ProfilerEvents.Names[eventClass]
        : "Event " + eventClass;
    }

    private static string GetFormattedValue(ProfilerEvent evt, int column, string format) {
      return ProfilerEventColumns.Duration == column
        ? (evt.Duration / 1000).ToString(format)
        : evt.GetFormattedData(column, format);
    }

    private int FocusedIndex => lvEvents.FocusedItem?.Index ?? -1;

    private void FocusRow(int index, bool ensureVisible) {
      var item = lvEvents.Items[index];
      item.Focused = true;
      item.Selected = true;
      UpdateSourceBox();
      if (ensureVisible)
        lvEvents.EnsureVisible(index);
    }

    private void SelectAllEvents(bool select) {
      lvEvents.BeginUpdate();
      _dontUpdateSource = true;
      try {
        if (select) {
          for (var i = 0; i < _rows.Count; i++)
            lvEvents.SelectedIndices.Add(i);
        }
        else {
          lvEvents.SelectedIndices.Clear();
        }
      }
      finally {
        _dontUpdateSource = false;
        UpdateSourceBox();
        lvEvents.EndUpdate();
      }
    }

    private void ShowSelectedEvent() {
      var focusedIndex = FocusedIndex;
      if (focusedIndex > -1 && focusedIndex < _rows.Count) {
        SelectAllEvents(false);
        FocusRow(focusedIndex, true);
      }
    }

    private void UpdateSourceBox() {
      if (_dontUpdateSource) return;

      var selectedIndices = lvEvents.SelectedIndices.Cast<int>().ToArray();
      if (_prevSelectedIndices is not null && selectedIndices.SequenceEqual(_prevSelectedIndices))
        return;
      _prevSelectedIndices = selectedIndices;

      var sb = new StringBuilder();
      foreach (var i in selectedIndices) {
        if (i >= _rows.Count) continue;
        var text = _rows[i].Event.TextData;
        if (text != "")
          sb.AppendFormat("{0}\r\ngo\r\n", FormatForDisplay(text));
      }

      _lexer.FillRichEdit(reTextData, sb.ToString());
      HighlightSearchInSource();
    }

    /// <summary>Replaces the rows and resets everything that depends on their positions.</summary>
    private void ReplaceRows(IEnumerable<EventRow> rows) {
      var list = rows.ToList();
      _rows.Clear();
      _rows.AddRange(list);
      lvEvents.VirtualListSize = _rows.Count;
      lvEvents.SelectedIndices.Clear();
      _prevSelectedIndices = null;
      lvEvents.Invalidate();
      UpdateSourceBox();
      UpdateEventsStatus();
    }

    private void AppendEvents(IEnumerable<ProfilerEvent> events) {
      foreach (var evt in events) {
        _eventCount++;
        var isMarker = evt == _startedEvent || evt == _pausedEvent || evt == _stoppedEvent;
        var row = new EventRow(evt, _eventCount, GetEventCaption(evt), isMarker);
        if (_filtering) {
          _unfilteredRows.Add(row);
          if (!IsIncludedByFilters(row)) continue;
        }

        _rows.Add(row);
      }

      TrimRows();
      lvEvents.VirtualListSize = _rows.Count;
      if (autoScrollToolStripMenuItem.Checked && _rows.Count > 0) {
        lvEvents.SelectedIndices.Clear();
        FocusRow(_rows.Count - 1, true);
      }

      lvEvents.Invalidate();
      UpdateEventsStatus();
    }

    private bool TrimRows() {
      var max = Math.Max(_traceSettings.Filters.MaximumEventCount, 0);
      if (_rows.Count <= max) return false;
      _trimmedCount += _rows.Count - max;
      _rows.RemoveRange(0, _rows.Count - max);
      // the events hidden by the filter are kept for a while, but not without a limit
      var maxUnfiltered = Math.Max(max, 1) * 4;
      if (_unfilteredRows.Count > maxUnfiltered)
        _unfilteredRows.RemoveRange(0, _unfilteredRows.Count - maxUnfiltered);
      _prevSelectedIndices = null;
      return true;
    }

    #endregion

    #region trace

    private void StartProfiling() {
      try {
        Cursor = Cursors.WaitCursor;
        _rateMeter.Reset();
        if (_session.State == ProfilingStateEnum.psPaused) {
          _session.Resume();
          _traceClock.Start();
        }
        else {
          _session.Start(_traceSettings);
          _activeServer = edServer.Text;
          RememberServer(_activeServer);
          _traceClock.Restart();
          _trimmedCount = 0;
          InitGridColumns();
          _eventCount = 0;
          ResetCapturedFilter();
          ReplaceRows(Enumerable.Empty<EventRow>());
        }

        AppendEvents(new[] {_startedEvent});
      }
      catch (SqlException e) when (IsPermissionError(e)) {
        MessageBox.Show(e.Message + "\r\n\r\nThe login needs the ALTER TRACE permission on the server (or the sysadmin role).",
          "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch (Exception e) {
        AppLogger.Error("Failed to start the profiling", e);
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

    private void TbStop_Click(object sender, EventArgs e) {
      StopProfiling();
    }

    private void StopProfiling() {
      tbStop.Enabled = false;
      try {
        _session.Stop();
      }
      catch (Exception e) {
        MessageBox.Show("The trace is stopped, but it could not be removed on the server:\r\n" + e.Message,
          "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      _traceClock.Stop();
      ApplySort();
      AppendEvents(new[] {_stoppedEvent});
      UpdateButtons();
    }

    private void TbPause_Click(object sender, EventArgs e) {
      PauseProfiling();
    }

    private void PauseProfiling() {
      try {
        _session.Pause();
      }
      catch (Exception e) {
        AppLogger.Error("Failed to pause the trace", e);
        MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      _traceClock.Stop();
      AppendEvents(new[] {_pausedEvent});
      UpdateButtons();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      if (_session.State == ProfilingStateEnum.psStopped) return;
      if (MessageBox.Show("There are traces still running. Are you sure you want to close the application?",
            "SqlProfiler", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
        StopProfiling();
      }
      else {
        e.Cancel = true;
      }
    }

    private void Timer1_Tick(object sender, EventArgs e) {
      var events = _session.TakeEvents();
      var error = _session.TakeError();

      if (events.Count > 0) {
        AppendEvents(events);
      }
      else if (TrimRows()) {
        lvEvents.VirtualListSize = _rows.Count;
        lvEvents.Invalidate();
        UpdateEventsStatus();
      }

      _slElapsed.Text = FormatElapsed(_traceClock.Elapsed);
      var rate = _rateMeter.Update(_eventCount);
      if (rate is not null)
        slEPS.Text = rate;

      if (error is not null) {
        StopProfiling();
        MessageBox.Show("Reading of the trace events failed, the trace is stopped:\r\n" + error.Message +
                        "\r\n\r\nDetails are written to " + AppLogger.FilePath,
          "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    internal void RunProfiling(bool showFilters) {
      if (showFilters) {
        using var frm = new TraceProperties();
        frm.SetSettings(_traceSettings.GetCopy());
        if (DialogResult.OK != frm.ShowDialog()) return;
        _traceSettings = frm.Settings.GetCopy();
      }

      StartProfiling();
    }

    private void TbRunWithFilters_Click(object sender, EventArgs e) {
      RunProfiling(true);
    }

    private void TbAuth_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateButtons();
    }

    #endregion

    #region captured events

    private void ClearTrace() {
      _trimmedCount = 0;
      ResetCapturedFilter();
      ReplaceRows(Enumerable.Empty<EventRow>());
    }

    private void TbClear_Click(object sender, EventArgs e) {
      ClearTrace();
    }

    private void ClearTraceWindowToolStripMenuItem_Click(object sender, EventArgs e) {
      ClearTrace();
    }

    private void DeleteSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
      var selected = new HashSet<int>(lvEvents.SelectedIndices.Cast<int>());
      ReplaceRows(_rows.Where((_, i) => !selected.Contains(i)));
    }

    private void KeepSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
      var selected = new HashSet<int>(lvEvents.SelectedIndices.Cast<int>());
      ReplaceRows(_rows.Where((_, i) => selected.Contains(i)));
    }

    private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e) {
      if (lvEvents.Focused && _session.State != ProfilingStateEnum.psProfiling) {
        SelectAllEvents(true);
      }
      else if (reTextData.Focused) {
        reTextData.SelectAll();
      }
    }

    private void SetFilterEvents() {
      if (_filtering) return;

      lvEvents.SelectedIndices.Clear();
      using var frm = new TraceProperties();
      frm.SetSettings(_traceSettings.GetCopy());
      if (DialogResult.OK != frm.ShowDialog()) {
        tbFilterEvents.Checked = false;
        return;
      }

      _traceSettings = frm.Settings.GetCopy();
      ApplyCapturedFilter(_traceSettings.Filters.Clone());
    }

    /// <summary>Whether the row passes the filter of the dialog and the quick filter. Markers (started, paused, stopped) always pass.</summary>
    private bool IsIncludedByFilters(EventRow row) {
      if (row.IsMarker) return true;
      return (_activeFilters is null || EventFilter.IsIncluded(_activeFilters, row)) && _quickFilter.Matches(row.Event);
    }

    /// <summary>The filter of the dialog: only the captured events that match it are shown.</summary>
    private void ApplyCapturedFilter(TraceProperties.TraceFilters filters) {
      _activeFilters = filters;
      ApplyFilters();
    }

    /// <summary>Adds one more condition to the quick filter (the lists of the values to show and to hide).</summary>
    private void ApplyQuickFilter(Action<QuickFilter> addCondition) {
      addCondition(_quickFilter);
      ApplyFilters();
    }

    /// <summary>Shows only the captured events that pass the filters; the others are kept until the filters are cleared.</summary>
    private void ApplyFilters() {
      if (!_filtering) {
        _filtering = true;
        _unfilteredRows.AddRange(_rows);
      }

      tbFilterEvents.Checked = true;
      var max = Math.Max(_traceSettings.Filters.MaximumEventCount, 0);
      ReplaceRows(_unfilteredRows.Where(IsIncludedByFilters).Take(max));
      ShowSelectedEvent();
    }

    private void ClearFilterEvents() {
      if (!_filtering) return;
      var rows = _unfilteredRows.ToList();
      ResetCapturedFilter();
      ReplaceRows(rows);
      if (TrimRows()) {
        lvEvents.VirtualListSize = _rows.Count;
        lvEvents.Invalidate();
        UpdateEventsStatus();
      }

      ShowSelectedEvent();
    }

    private void ResetCapturedFilter() {
      _filtering = false;
      _activeFilters = null;
      _quickFilter.Clear();
      _unfilteredRows.Clear();
      tbFilterEvents.Checked = false;
      UpdateEventsStatus();
    }

    private void FilterCapturedEventsToolStripMenuItem_Click(object sender, EventArgs e) {
      SetFilterEvents();
    }

    private void ClearCapturedFiltersToolStripMenuItem_Click(object sender, EventArgs e) {
      ClearFilterEvents();
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

    #endregion

    #region find

    private void FindToolStripMenuItem_Click(object sender, EventArgs e) {
      ShowQuickSearch();
    }

    private void FindNextToolStripMenuItem_Click(object sender, EventArgs e) {
      PerformFind(true, true);
    }

    internal void PerformFind(bool forwards, bool wrapAround) {
      if (string.IsNullOrEmpty(LastPattern)) return;
      var index = EventSearch.Find(_rows, LastPattern, MatchCase, WholeWord, forwards, wrapAround, FocusedIndex);
      if (index < 0) {
        MessageBox.Show($"Failed to find \"{LastPattern}\". Searched to the end of data. ", "SqlProfiler",
          MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      SelectFoundRow(index);
    }

    private void SelectFoundRow(int index) {
      // the list must not jump to the newest event right after the found one is selected
      if (_session.State == ProfilingStateEnum.psProfiling && autoScrollToolStripMenuItem.Checked)
        autoScrollToolStripMenuItem.Checked = false;

      SelectAllEvents(false);
      FocusRow(index, true);
    }

    #endregion

    #region export

    private void CopyAllToClipboardToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(false);
    }

    private void CopySelectedToClipboardToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(true);
    }

    private void ExtractAllEventsToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(false);
    }

    private void ExtractSelectedEventsToolStripMenuItem_Click(object sender, EventArgs e) {
      CopyEventsToClipboard(true);
    }

    private void CopyEventsToClipboard(bool copySelected) {
      var rows = copySelected
        ? lvEvents.SelectedIndices.Cast<int>().Select(i => _rows[i])
        : _rows;
      Clipboard.SetText(EventExporter.BuildEventsXml(rows.Select(r => r.Event).ToList()));
      MessageBox.Show("Event(s) data copied to clipboard", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void CopyToXlsToolStripMenuItem_Click(object sender, EventArgs e) {
      var doc = BuildExcelDocument();
      using var xmlStream = new MemoryStream(EventExporter.GetXmlBytes(doc));
      Clipboard.SetData("XML Spreadsheet", xmlStream);
      MessageBox.Show("Event(s) data copied to clipboard", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SaveAllEventsToExcelXmlFileToolStripMenuItem_Click(object sender, EventArgs e) {
      var doc = BuildExcelDocument();
      using var sfd = new SaveFileDialog {Filter = "Excel XML|*.xml", Title = "Save the Excel XML file"};
      if (sfd.ShowDialog(this) != DialogResult.OK || string.IsNullOrEmpty(sfd.FileName)) return;
      File.WriteAllBytes(sfd.FileName, EventExporter.GetXmlBytes(doc));
      MessageBox.Show($"File saved to: {sfd.FileName}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private XmlDocument BuildExcelDocument() {
      return EventExporter.BuildExcelDocument(lvEvents, _columns, _rows.Select(r => r.Event).ToList(), GetEventCaption, GetFormattedValue);
    }

    #endregion

    #region menu and connection fields

    private void PauseTraceToolStripMenuItem_Click(object sender, EventArgs e) {
      PauseProfiling();
    }

    private void StopTraceToolStripMenuItem_Click(object sender, EventArgs e) {
      StopProfiling();
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    private void MnAbout_Click(object sender, EventArgs e) {
      Updater.ShowAbout();
    }

    private void StayOnTop_Click(object sender, EventArgs e) {
      stayOnTopToolStripMenuItem.Checked = !stayOnTopToolStripMenuItem.Checked;
      TopMost = stayOnTopToolStripMenuItem.Checked;
    }

    protected override void OnMouseWheel(MouseEventArgs e) {
      if (e.Button != MouseButtons.None) return;
      var delta = e.Delta > 0 ? 0.05 : -0.05;
      var nextOpacity = Opacity + delta;
      if (nextOpacity < 0.1) nextOpacity = 0.1;
      if (nextOpacity > 1) nextOpacity = 1;
      Opacity = nextOpacity;
    }

    private void EdServer_TextChanged(object sender, EventArgs e) {
      _settings.SetValue("Server", edServer.Text);
    }

    private void EdUser_TextChanged(object sender, EventArgs e) {
      _settings.SetValue("User", edUser.Text);
    }

    private void EdPassword_TextChanged(object sender, EventArgs e) {
      _settings.SetValue("Password", edPassword.Text);
    }

    private void SiteToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.VisitAppSite();
    }

    private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.CheckForUpdates(Updater.CheckUpdatesMode.AllMessages);
    }

    #endregion
  }
}
