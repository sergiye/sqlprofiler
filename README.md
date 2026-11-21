# SqlProfiler
[![Release](https://img.shields.io/github/v/release/sergiye/sqlprofiler)](https://github.com/sergiye/sqlprofiler/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/sergiye/sqlprofiler/total?color=ff4f42)](https://sergiye.github.io/github-release-stats/?username=sergiye&repository=sqlprofiler&page=1&per_page=100)
![Last commit](https://img.shields.io/github/last-commit/sergiye/sqlprofiler?color=00AD00)

*SqlProfiler is a simple and fast replacement for SQL Server Profiler with basic GUI.*

Can be used with both LocalDB, Express, Development and full-functional editions of SQL Server (2005 and later).
Distribution package contains standalone portable (can be used without installation) version of SqlProfiler.

## What does it look like?

Here's a preview of the app's UI running on Windows 10:

[<img src="https://github.com/sergiye/sqlprofiler/raw/master/preview.png" alt="preview" width="300"/>](https://github.com/sergiye/sqlprofiler/raw/master/preview.png)

## Download Latest Version

The published version can be obtained from [releases](https://github.com/sergiye/sqlprofiler/releases) page, or get the newer one directly from:
[Latest Version](https://github.com/sergiye/sqlprofiler/releases/latest)

## Requirements
 * Windows with .NET Framework 4.8
 * SQL Server 2005 or later (LocalDB, Express, Developer, Standard, Enterprise)
 * A login with the `ALTER TRACE` permission on the server (membership in `sysadmin` also works)

## Features

### Tracing
 * Server-side trace created via `sp_trace_create`/`sp_trace_setevent`, events are streamed to the app with `sp_trace_getdata`
 * Windows (default) or SQL Server authentication
 * Recent servers (drop-down list next to the server name and auto complete), the `Test` button checks the connection and the `ALTER TRACE` permission
 * `Encrypt connection` and `Trust server certificate` options in the File menu (the certificate is trusted by default)
 * Start, pause/resume and stop a running trace; if the connection is lost the app offers to start a new trace with the same settings and keeps the events that are already in the list
 * "Run with filters" opens the trace properties dialog before the start
 * Presets: save the current events, columns and filters under a name and apply them before the next trace (Presets button)
 * Selectable events:
   * `SQL:BatchStarting`, `SQL:BatchCompleted`
   * `SQL:StmtStarting`, `SQL:StmtCompleted`
   * `RPC:Starting`, `RPC:Completed`
   * `SP:StmtStarting`, `SP:StmtCompleted`
   * `Audit Login`/`Audit Logout`, `ExistingConnection`
   * `User Error Message`, `Blocked process report`
 * Optional columns: Start time, End time, Database name, Object name, Application name, Host name (in addition to Event Class, Text Data, Login Name, CPU, Reads, Writes, Duration, SPID that are always shown). All of them are captured, so they can be shown or hidden at any time
 * Events produced by SqlProfiler itself are excluded automatically
 * Status bar: state of the trace, server, running time, number of events in the list (and how many of the oldest ones were removed by the limit), events per second (last/average) and the active filter
 * The list keeps only the latest N events (5000 by default, configurable)

### Filters
 * Server-side filters applied while tracing: CPU, Reads, Writes, Duration, SPID (`=`, `<>`, `>`, `<`) and Login name, Host name, Database name, Application name, Text data (`like`/`not like`)
 * Maximum events count
 * "Filter Captured Events" applies the same filters to events that are already captured, "Clear Captured Filters" restores the full list

### Working with the captured events
 * Top queries (View - Top queries, `Ctrl+Q`): the events grouped by query (values replaced with `?`) with the count and the total/average/max duration, CPU, reads and writes; sort by any column, select all the events of a query in the list
 * Right click on the column header: show or hide the Start/End time, Database, Object, Application and Host columns, reset the widths
 * Sorting is applied when the trace stops; View - "Keep sorted while tracing" sorts the new events at once. The selected rows stay selected after sorting
 * Virtual list with thousands of events, sortable by any column (numeric columns are sorted as numbers)
 * Text data of selected events is shown in the bottom panel with T-SQL syntax highlighting; several events can be selected at once; the toolbar of the panel: "Format SQL" puts the clauses on separate lines (also inside `sp_executesql` and one argument of an `exec` per line), "Copy", "Wrap", "Lines" (line numbers), "A-"/"A+" (text size, also `Ctrl`+mouse wheel)
 * Quick search bar (`Ctrl+F`): finds while you type, highlights the matches in the bottom panel, shows "n of N", Enter/`F3` - next, Shift+Enter/`Shift+F3` - previous, match case and whole word options, `Esc` closes; works while the trace is running (auto scroll is switched off then)
 * Select all (also while the trace is running, auto scroll is switched off then), delete selected, keep only selected events, clear the trace window
 * Auto scroll to the latest event
 * Rows of errors (`User Error Message`) and blocked process reports are highlighted; slow events can be highlighted by a threshold (View - Highlight slow events)
 * Context menu of a row: copy text, copy as one line, search for this text (or for the text selected in the bottom panel), show only this SPID, show only/hide the application, login, database or host of the event (the values add up: hide several applications one after another; the active filter is shown in the status bar, a click on it clears the filter)

### Export
 * Copy all/selected events to clipboard as XML
 * Copy all events to clipboard in Excel's XML Spreadsheet format
 * Save all events to an Excel XML file
 * Save all events as CSV or JSON (raw values of all the columns, UTF-8)
 * Save the captured events to a compressed trace file (`.sptrace`) and open it later (File menu)

### Appearance and settings
 * `Auto`/`Light`/`Dark` themes integrated into executable, `Auto` follows the system theme
 * Custom themes from external files (see below)
 * Stay on top (`Ctrl+T`), window opacity is changed with the mouse wheel over the window
 * The default widths of the columns follow the DPI of the screen and are narrowed to fit the list
 * Window position, size and state, the splitter, the widths of the columns, the selected events, columns and filters of the trace, the sorting, connection settings, search options and the selected theme are remembered
 * Settings can be stored in a file next to the executable (portable mode) or in the user profile
 * Built-in update check (`Ctrl+U`)
 * Errors are logged to `SqlProfiler.log` next to the executable (Help - Open log file), nothing is written elsewhere

### Keyboard shortcuts
 * `F5` or `Alt+S` start/resume, `Alt+P` pause, `Shift+F5` or `Alt+T` stop trace
 * `Ctrl+F` quick search bar, `F3`/`Shift+F3` next/previous match, `Ctrl+A` select all
 * `Ctrl+Q` top queries
 * `Ctrl+C` copy the text of the selected queries (separated with `go`)
 * `Ctrl+Delete` delete selected, `Alt+Delete` keep selected, `Ctrl+Shift+Delete` clear trace window
 * `Ctrl+S` auto scroll, `Ctrl+T` stay on top
 * `Ctrl+U` check for updates, `F1` about, `Ctrl+F1` open site, `Ctrl+W` exit

## Custom themes

To add custom theme to the app, just create a `themes` folder next to the executable file and place any {themeName}.json files there.
Custom theme.json file content example:
```json
{
  "DisplayName": "Custom Theme",
  "DarkMode": true,
  "BackgroundColor": "#1E1E1E",
  "ForegroundColor": "#E9E9E9",
  "HyperlinkColor": "#00D980",
  "SelectedBackgroundColor": "#4CBB17",
  "SelectedForegroundColor": "#000000",
  "LineColor": "#262626",
  "StrongLineColor": "#454545",
  "WarnColor": "#FF4500"
}
```
Don't forget to restart the app to scan for new theme files!

## Command-line parameters
Can be used to set default GUI values.

Connection:
 * `-server`, `-s` `<server name>`
 * `-user`, `-u` `<user name>`
 * `-password`, `-p` `<user password>`
 * `-start` : automatically start profiling
 * `-preset` `<name>` : use the events, columns and filters of a saved preset (the arguments after it can still change them)

Limits:
 * `-maxevents`, `-m` `<value>` : maximum events in event list (5000 by default)
 * `-duration`, `-d` `<min duration, ms>` : sets filter on duration column

Events (all of them are off by default unless selected in the trace properties):
 * `-batchcompleted`, `-batchstarting`
 * `-rpccompleted`, `-rpcstarting`
 * `-spstmtcompleted`, `-spstmtstarting`
 * `-existingconnection`, `-loginlogout`

Filters, `-<column> <condition> <value>`:
 * numeric columns `-cpu`, `-duration`, `-reads`, `-writes`, `-spid` with conditions `equal`/`eq`/`=`, `notequal`/`ne`/`!=`/`<>`, `greaterthan`/`ge`/`>`, `lessthan`/`le`/`<`
 * text columns `-databasename`, `-loginname`, `-hostname`, `-textdata`, `-applicationname` with conditions `like`/`eq`/`=`, `notlike`

Example:
```
SqlProfiler.exe -s .\sqlexpress -u sa -p secret -batchcompleted -rpccompleted -duration > 100 -applicationname like MyApp -start
```

## How can I help improve it?
The sqlprofiler team welcomes feedback and contributions!<br/>
You can check if it works properly on your PC. If you notice any inaccuracies, please send us a pull request. If you have any suggestions or improvements, don't hesitate to create an issue.

Also, don't forget to ★ star ★ the repository to help other people find it.

## Donate
If you find this project useful, consider [supporting the author](https://patreon.com/SergiyE).

## License

This program is free to use for **personal, home and other non-commercial purposes only**.

Any commercial use is not allowed without prior written permission from the author. This includes use inside a company or organization, bundling it with or into commercial products or services, and selling it or charging for access to it. For commercial licensing, please contact the author through [GitHub issues](https://github.com/sergiye/sqlprofiler/issues).

You may share unmodified copies of the program free of charge, as long as they are used under the same terms.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

