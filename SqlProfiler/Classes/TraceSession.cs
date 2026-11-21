using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace SqlProfiler {
  /// <summary>One server-side trace: creates it, reads its events in the background, pauses, resumes and stops it.</summary>
  internal sealed class TraceSession : IDisposable {
    private static readonly TimeSpan ReaderStopTimeout = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan ReaderForcedStopTimeout = TimeSpan.FromSeconds(2);

    private readonly Func<SqlConnection> _connectionFactory;
    private readonly object _sync = new();
    private List<ProfilerEvent> _events = new();
    private Exception _error;

    private SqlConnection _connection;
    private RawTraceReader _reader;
    private CancellationTokenSource _cts;
    private Task _readerTask;

    public TraceSession(Func<SqlConnection> connectionFactory) {
      _connectionFactory = connectionFactory;
    }

    public ProfilingStateEnum State { get; private set; } = ProfilingStateEnum.psStopped;

    /// <summary>Creates the trace on the server, configures it and starts to read its events.</summary>
    public void Start(TraceProperties.TraceSettings settings) {
      if (State != ProfilingStateEnum.psStopped)
        throw new InvalidOperationException("The trace is already running.");

      try {
        _connection = _connectionFactory();
        _connection.Open();
        _reader = new RawTraceReader(_connection);
        _reader.CreateTrace();
        TraceConfigurator.Apply(_reader, settings);
        lock (_sync) {
          _events = new List<ProfilerEvent>();
          _error = null;
        }

        StartReading();
        AppLogger.Info($"Trace {_reader.TraceId} started");
      }
      catch (Exception e) {
        AppLogger.Error("Failed to start the trace", e);
        Cleanup(closeServerTrace: true);
        throw;
      }
    }

    public void Pause() {
      if (State != ProfilingStateEnum.psProfiling) return;
      using (var connection = _connectionFactory()) {
        connection.Open();
        _reader.StopTrace(connection);
      }

      State = ProfilingStateEnum.psPaused;
      AppLogger.Info($"Trace {_reader.TraceId} paused");
    }

    public void Resume() {
      if (State != ProfilingStateEnum.psPaused) return;
      StartReading();
      AppLogger.Info($"Trace {_reader.TraceId} resumed");
    }

    /// <summary>Stops and removes the trace on the server and releases everything. The session is stopped even if the server cannot be reached; the error is thrown then.</summary>
    public void Stop() {
      if (_reader is null) {
        State = ProfilingStateEnum.psStopped;
        return;
      }

      var traceId = _reader.TraceId;
      Exception serverError = null;
      try {
        using var connection = _connectionFactory();
        connection.Open();
        _reader.StopTrace(connection);
        _reader.CloseTrace(connection);
      }
      catch (Exception e) {
        AppLogger.Error($"Failed to stop the trace {traceId} on the server", e);
        serverError = e;
      }

      Cleanup(closeServerTrace: false);
      AppLogger.Info($"Trace {traceId} stopped");
      if (serverError is not null)
        ExceptionDispatchInfo.Capture(serverError).Throw();
    }

    /// <summary>The events received since the previous call.</summary>
    public List<ProfilerEvent> TakeEvents() {
      lock (_sync) {
        var events = _events;
        _events = new List<ProfilerEvent>();
        return events;
      }
    }

    /// <summary>The error that interrupted reading of the events, once.</summary>
    public Exception TakeError() {
      lock (_sync) {
        var error = _error;
        _error = null;
        return error;
      }
    }

    public void Dispose() {
      try {
        Stop();
      }
      catch {
        // already logged
      }
    }

    private void StartReading() {
      StopReading();
      if (_connection.State != ConnectionState.Open)
        _connection.Open();
      _reader.StartTrace();
      _cts = new CancellationTokenSource();
      var token = _cts.Token;
      State = ProfilingStateEnum.psProfiling;
      _readerTask = Task.Factory.StartNew(() => ReadEvents(token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private void StopReading() {
      if (_cts is not null) _cts.Cancel();
      _reader?.Close();

      if (_readerTask is not null && !_readerTask.Wait(ReaderStopTimeout)) {
        // the read is blocked on the server: closing the connection interrupts it
        AppLogger.Warn($"The reader of the trace did not stop in {ReaderStopTimeout.TotalSeconds:0} s, closing the connection");
        try {
          _connection?.Close();
        }
        catch (Exception e) {
          AppLogger.Error("Failed to close the connection", e);
        }

        if (!_readerTask.Wait(ReaderForcedStopTimeout))
          AppLogger.Warn("The reader of the trace is still running, it is abandoned");
      }

      _readerTask = null;
      _cts?.Dispose();
      _cts = null;
    }

    private void Cleanup(bool closeServerTrace) {
      var reader = _reader;
      var connection = _connection;
      try {
        if (closeServerTrace && reader is not null && connection is not null && connection.State == ConnectionState.Open)
          reader.CloseTrace(connection);
      }
      catch (Exception e) {
        AppLogger.Error("Failed to remove the trace from the server", e);
      }

      try {
        StopReading();
      }
      catch (Exception e) {
        AppLogger.Error("Failed to stop the reader of the trace", e);
      }

      connection?.Dispose();
      _connection = null;
      _reader = null;
      State = ProfilingStateEnum.psStopped;
    }

    private void ReadEvents(CancellationToken token) {
      var reader = _reader;
      try {
        while (!token.IsCancellationRequested && reader.TraceIsActive) {
          var evt = reader.Next();
          if (evt is null) continue;
          lock (_sync) {
            _events.Add(evt);
          }
        }
      }
      catch (Exception e) {
        if (token.IsCancellationRequested || !reader.TraceIsActive) return;
        AppLogger.Error("Reading of the trace events failed", e);
        lock (_sync) {
          _error = e;
        }
      }
    }
  }
}
