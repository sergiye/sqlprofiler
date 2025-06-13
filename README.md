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

Also there are:
 - `Auto`/`Light`/`Dark` themes integrated into executable.
 - Custom `themes` supported from external files

You can find custom theme examples [here](https://github.com/sergiye/sqlprofiler/tree/master/themes)
To add custom theme to the app, just create a `themes` folder next to the executable file and place all theme files there.
Don't forget to restart the app to scan for new theme files!

## Download Latest Version

The published version can be obtained from [releases](https://github.com/sergiye/sqlprofiler/releases) page, or get the newer one directly from:
[Latest Version](https://github.com/sergiye/sqlprofiler/releases/latest)

## Features
 * Tracing of basic set of events (Batch/RPC/SP:Stmt Starting/Completed, Audit login/logout, User error messages, Blocked Process report) and columns (Event Class, Text Data,Login, CPU, Reads, Writes, Duration, SPID, Start/End time, Database/Object/Application name) - both selectable
 * Filters on most data columns
 * Copy all/selected event rows to clipboard in form of XML 
 * Find in "Text data" column
 * Export data in Excel's clipboard format


## Command-line parameters 
Can be used to set default GUI values
 * -server, -s  <server name>
 * -user, -u  <user name>
 * -password, -p <user password>
 * -maxevents, -m <value> : set maximum events in event list (1000 by default)
 * -duration, -d <min duration> : sets filter on duration column
 * -start  :  automatically start profiling

## How can I help improve it?
The sqlprofiler team welcomes feedback and contributions!<br/>
You can check if it works properly on your PC. If you notice any inaccuracies, please send us a pull request. If you have any suggestions or improvements, don't hesitate to create an issue.

Also, don't forget to star the repository to help other people find it.

[![Star History Chart](https://api.star-history.com/svg?repos=sergiye/sqlprofiler&type=Date)](https://star-history.com/#sergiye/sqlprofiler&Date)

[![Stargazers repo roster for @sergiye/sqlprofiler](https://reporoster.com/stars/sergiye/sqlprofiler)](https://github.com/sergiye/sqlprofiler/stargazers)

## Donate!
Every [cup of coffee](https://patreon.com/SergiyE) you donate will help this app become better and let me know that this project is in demand.

## License

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

