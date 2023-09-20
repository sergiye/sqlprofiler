# SqlProfiler

![GitHub release (latest by date)](https://img.shields.io/github/v/release/sergiye/sqlprofiler?style=plastic)
![GitHub all releases](https://img.shields.io/github/downloads/sergiye/sqlprofiler/total?style=plastic)
![GitHub last commit](https://img.shields.io/github/last-commit/sergiye/sqlprofiler?style=plastic)

*SqlProfiler is a simple and fast replacement for SQL Server Profiler with basic GUI.*

Can be used with both LocalDB, Express, Development and full-functional editions of SQL Server (2005 and later).
Distribution package contains standalone portable (can be used without installation) version of SqlProfiler.

## Download Latest Version

Authenticator provides an alternative solution to combine various two-factor authenticator services in one convenient place.

This is the latest stable version and can be downloaded from the [releases](https://github.com/sergiye/sqlprofiler/releases) page, or get the newer one directly from:
[Latest Version](https://github.com/sergiye/sqlprofiler/releases/latest)

## Features
 * Tracing of basic set of events (Batch/RPC/SP:Stmt Starting/Completed, Audit login/logout, User error messages, Blocked Process report) and columns (Event Class, Text Data,Login, CPU, Reads, Writes, Duration, SPID, Start/End time, Database/Object/Application name) - both selectable
 * Filters on most data columns
 * Copy all/selected event rows to clipboard in form of XML 
 * Find in "Text data" column
 * Export data in Excel's clipboard format

# Build

**The recommended way to get the program is BUILD from source**
- Install git, Visual Studio (2022 or higher)
- `git clone https://github.com/sergiye/sqlprofiler.git`
- build


## Command-line parameters 
Can be used to set default GUI values
 * -server, -s  <server name>
 * -user, -u  <user name>
 * -password, -p <user password>
 * -maxevents, -m <value> : set maximum events in event list (1000 by default)
 * -duration, -d <min duration> : sets filter on duration column
 * -start  :  automatically start profiling

## License

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License  along with this program.  If not, see http://www.gnu.org/licenses/.

