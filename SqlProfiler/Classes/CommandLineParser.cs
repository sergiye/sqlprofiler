using System;
using System.Collections.Generic;

namespace SqlProfiler {
  internal static class CommandLineParser {
    private static readonly Dictionary<string, Action<TraceProperties.TraceEventsColumns>> EventFlags =
      new Dictionary<string, Action<TraceProperties.TraceEventsColumns>> {
        ["-batchcompleted"] = e => e.BatchCompleted = true,
        ["-batchstarting"] = e => e.BatchStarting = true,
        ["-existingconnection"] = e => e.ExistingConnection = true,
        ["-loginlogout"] = e => e.LoginLogout = true,
        ["-rpccompleted"] = e => e.RPCCompleted = true,
        ["-rpcstarting"] = e => e.RPCStarting = true,
        ["-spstmtcompleted"] = e => e.SPStmtCompleted = true,
        ["-spstmtstarting"] = e => e.SPStmtStarting = true,
      };

    private static readonly Dictionary<string, Action<TraceProperties.TraceFilters, int, TraceProperties.IntFilterCondition>> IntFilters =
      new Dictionary<string, Action<TraceProperties.TraceFilters, int, TraceProperties.IntFilterCondition>> {
        ["-cpu"] = (f, v, c) => { f.CPU = v; f.CpuFilterCondition = c; },
        ["-duration"] = (f, v, c) => { f.Duration = v; f.DurationFilterCondition = c; },
        ["-reads"] = (f, v, c) => { f.Reads = v; f.ReadsFilterCondition = c; },
        ["-writes"] = (f, v, c) => { f.Writes = v; f.WritesFilterCondition = c; },
        ["-spid"] = (f, v, c) => { f.SPID = v; f.SPIDFilterCondition = c; },
      };

    private static readonly Dictionary<string, Action<TraceProperties.TraceFilters, string, TraceProperties.StringFilterCondition>> StringFilters =
      new Dictionary<string, Action<TraceProperties.TraceFilters, string, TraceProperties.StringFilterCondition>> {
        ["-databasename"] = (f, v, c) => { f.DatabaseName = v; f.DatabaseNameFilterCondition = c; },
        ["-loginname"] = (f, v, c) => { f.LoginName = v; f.LoginNameFilterCondition = c; },
        ["-hostname"] = (f, v, c) => { f.HostName = v; f.HostNameFilterCondition = c; },
        ["-textdata"] = (f, v, c) => { f.TextData = v; f.TextDataFilterCondition = c; },
        ["-applicationname"] = (f, v, c) => { f.ApplicationName = v; f.ApplicationNameFilterCondition = c; },
      };

    /// <summary>Applies the arguments to the trace settings. Returns true if the trace has to be started at once.</summary>
    public static bool Apply(string[] args, TraceProperties.TraceSettings settings, Action<string, string> setSetting) {
      var autoStart = false;
      var i = 1;
      while (i < args.Length) {
        var name = args[i].ToLowerInvariant();
        var next = i + 1 < args.Length ? args[i + 1] : "";
        switch (name) {
          case "-s":
          case "-server":
            setSetting("Server", next);
            i++;
            break;
          case "-u":
          case "-user":
            setSetting("User", next);
            i++;
            break;
          case "-p":
          case "-password":
            setSetting("Password", next);
            i++;
            break;
          case "-m":
          case "-maxevents":
            if (int.TryParse(next, out var maxEvents)) i++;
            else maxEvents = 1000;
            settings.Filters.MaximumEventCount = maxEvents;
            break;
          case "-d":
          case "-duration":
            if (int.TryParse(next, out var duration)) {
              settings.Filters.DurationFilterCondition = TraceProperties.IntFilterCondition.GreaterThan;
              settings.Filters.Duration = duration;
              i++;
              break;
            }

            // "-duration <condition> <value>" is a regular filter
            if (name == "-duration") goto default;
            break;
          case "-start":
            autoStart = true;
            break;
          default:
            if (EventFlags.TryGetValue(name, out var setEvent)) {
              setEvent(settings.EventsColumns);
            }
            else if (IntFilters.TryGetValue(name, out var setInt)) {
              var value = i + 2 < args.Length ? args[i + 2] : "";
              setInt(settings.Filters, int.Parse(value), TraceProperties.ParseIntCondition(next));
              i += 2;
            }
            else if (StringFilters.TryGetValue(name, out var setString)) {
              var value = i + 2 < args.Length ? args[i + 2] : "";
              setString(settings.Filters, value, TraceProperties.ParseStringCondition(next));
              i += 2;
            }

            break;
        }

        i++;
      }

      return autoStart;
    }
  }
}
