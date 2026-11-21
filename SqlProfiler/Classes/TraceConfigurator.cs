using System;
using System.Collections.Generic;

namespace SqlProfiler {
  internal static class TraceConfigurator {
    private const string OwnApplicationName = "SqlProfiler";

    private static readonly int[] LoginColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName, ProfilerEventColumns.SPID,
      ProfilerEventColumns.StartTime, ProfilerEventColumns.EndTime, ProfilerEventColumns.HostName
    };

    private static readonly int[] LogoutColumns = {
      ProfilerEventColumns.CPU, ProfilerEventColumns.Reads, ProfilerEventColumns.Writes, ProfilerEventColumns.Duration,
      ProfilerEventColumns.LoginName, ProfilerEventColumns.SPID, ProfilerEventColumns.StartTime,
      ProfilerEventColumns.EndTime, ProfilerEventColumns.ApplicationName, ProfilerEventColumns.HostName
    };

    private static readonly int[] ExistingConnectionColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.SPID, ProfilerEventColumns.StartTime,
      ProfilerEventColumns.EndTime, ProfilerEventColumns.ApplicationName, ProfilerEventColumns.HostName
    };

    private static readonly int[] StartingColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName, ProfilerEventColumns.SPID,
      ProfilerEventColumns.StartTime, ProfilerEventColumns.EndTime, ProfilerEventColumns.DatabaseName,
      ProfilerEventColumns.ApplicationName, ProfilerEventColumns.HostName
    };

    private static readonly int[] CompletedColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName, ProfilerEventColumns.CPU,
      ProfilerEventColumns.Reads, ProfilerEventColumns.Writes, ProfilerEventColumns.Duration,
      ProfilerEventColumns.SPID, ProfilerEventColumns.StartTime, ProfilerEventColumns.EndTime,
      ProfilerEventColumns.DatabaseName, ProfilerEventColumns.ApplicationName, ProfilerEventColumns.HostName
    };

    private static readonly int[] RpcStartingColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName, ProfilerEventColumns.SPID,
      ProfilerEventColumns.StartTime, ProfilerEventColumns.EndTime, ProfilerEventColumns.DatabaseName,
      ProfilerEventColumns.ObjectName, ProfilerEventColumns.ApplicationName, ProfilerEventColumns.HostName
    };

    private static readonly int[] RpcCompletedColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName, ProfilerEventColumns.CPU,
      ProfilerEventColumns.Reads, ProfilerEventColumns.Writes, ProfilerEventColumns.Duration,
      ProfilerEventColumns.SPID, ProfilerEventColumns.StartTime, ProfilerEventColumns.EndTime,
      ProfilerEventColumns.DatabaseName, ProfilerEventColumns.ObjectName, ProfilerEventColumns.ApplicationName,
      ProfilerEventColumns.HostName
    };

    private static readonly int[] SpStatementColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName, ProfilerEventColumns.CPU,
      ProfilerEventColumns.Reads, ProfilerEventColumns.Writes, ProfilerEventColumns.Duration,
      ProfilerEventColumns.SPID, ProfilerEventColumns.StartTime, ProfilerEventColumns.EndTime,
      ProfilerEventColumns.DatabaseName, ProfilerEventColumns.ObjectName, ProfilerEventColumns.ObjectID,
      ProfilerEventColumns.ApplicationName, ProfilerEventColumns.HostName
    };

    private static readonly int[] MessageColumns = {
      ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName, ProfilerEventColumns.CPU,
      ProfilerEventColumns.SPID, ProfilerEventColumns.StartTime, ProfilerEventColumns.DatabaseName,
      ProfilerEventColumns.ApplicationName, ProfilerEventColumns.HostName
    };

    private static IEnumerable<KeyValuePair<int, int[]>> GetEvents(TraceProperties.TraceEventsColumns events) {
      if (events.LoginLogout) {
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.SecurityAudit.AuditLogin, LoginColumns);
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.SecurityAudit.AuditLogout, LogoutColumns);
      }

      if (events.ExistingConnection)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.Sessions.ExistingConnection, ExistingConnectionColumns);
      if (events.BatchCompleted)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.TSQL.SQLBatchCompleted, CompletedColumns);
      if (events.BatchStarting)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.TSQL.SQLBatchStarting, StartingColumns);
      if (events.RPCStarting)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.StoredProcedures.RPCStarting, RpcStartingColumns);
      if (events.RPCCompleted)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.StoredProcedures.RPCCompleted, RpcCompletedColumns);
      if (events.SPStmtCompleted)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.StoredProcedures.SPStmtCompleted, SpStatementColumns);
      if (events.SPStmtStarting)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.StoredProcedures.SPStmtStarting, SpStatementColumns);
      if (events.UserErrorMessage)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.ErrorsAndWarnings.UserErrorMessage, MessageColumns);
      if (events.BlockedProcessReport)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.ErrorsAndWarnings.Blockedprocessreport, MessageColumns);
      if (events.SQLStmtStarting)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.TSQL.SQLStmtStarting, CompletedColumns);
      if (events.SQLStmtCompleted)
        yield return new KeyValuePair<int, int[]>(ProfilerEvents.TSQL.SQLStmtCompleted, CompletedColumns);
    }

    public static void Apply(RawTraceReader reader, TraceProperties.TraceSettings settings) {
      foreach (var e in GetEvents(settings.EventsColumns))
        reader.SetEvent(e.Key, e.Value);

      var filters = settings.Filters;
      if (filters.Duration is not null)
        SetIntFilter(reader, filters.Duration * 1000L, filters.DurationFilterCondition, ProfilerEventColumns.Duration);
      SetIntFilter(reader, filters.Reads, filters.ReadsFilterCondition, ProfilerEventColumns.Reads);
      SetIntFilter(reader, filters.Writes, filters.WritesFilterCondition, ProfilerEventColumns.Writes);
      SetIntFilter(reader, filters.CPU, filters.CpuFilterCondition, ProfilerEventColumns.CPU);
      SetIntFilter(reader, filters.SPID, filters.SPIDFilterCondition, ProfilerEventColumns.SPID);

      SetStringFilter(reader, filters.LoginName, filters.LoginNameFilterCondition, ProfilerEventColumns.LoginName);
      SetStringFilter(reader, filters.HostName, filters.HostNameFilterCondition, ProfilerEventColumns.HostName);
      SetStringFilter(reader, filters.DatabaseName, filters.DatabaseNameFilterCondition, ProfilerEventColumns.DatabaseName);
      SetStringFilter(reader, filters.TextData, filters.TextDataFilterCondition, ProfilerEventColumns.TextData);
      SetStringFilter(reader, filters.ApplicationName, filters.ApplicationNameFilterCondition, ProfilerEventColumns.ApplicationName);

      reader.SetFilter(ProfilerEventColumns.ApplicationName, LogicalOperators.AND, ComparisonOperators.NotLike, OwnApplicationName);
    }

    private static void SetIntFilter(RawTraceReader reader, long? value, TraceProperties.IntFilterCondition condition, int column) {
      if (value is null) return;
      var op = condition switch {
        TraceProperties.IntFilterCondition.Equal => ComparisonOperators.Equal,
        TraceProperties.IntFilterCondition.NotEqual => ComparisonOperators.NotEqual,
        TraceProperties.IntFilterCondition.GreaterThan => ComparisonOperators.GreaterThan,
        TraceProperties.IntFilterCondition.LessThan => ComparisonOperators.LessThan,
        _ => throw new ArgumentOutOfRangeException(nameof(condition))
      };

      reader.SetFilter(column, LogicalOperators.AND, op, value);
    }

    private static void SetStringFilter(RawTraceReader reader, string value, TraceProperties.StringFilterCondition condition, int column) {
      if (string.IsNullOrEmpty(value)) return;
      reader.SetFilter(column, LogicalOperators.AND,
        condition == TraceProperties.StringFilterCondition.Like ? ComparisonOperators.Like : ComparisonOperators.NotLike,
        value);
    }
  }
}
