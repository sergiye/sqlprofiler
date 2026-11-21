using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SqlProfiler {
  /// <summary>Saves captured events to a file and reads them back. The file is compressed and keeps every column of every event.</summary>
  internal static class TraceFile {
    public const string Extension = ".sptrace";
    private const string Magic = "SPTRACE1";
    private const int ColumnCount = 66;

    public static void Save(string path, IReadOnlyCollection<ProfilerEvent> events) {
      using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
      using var gzip = new GZipStream(file, CompressionMode.Compress);
      using var writer = new BinaryWriter(gzip, Encoding.UTF8);
      writer.Write(Encoding.ASCII.GetBytes(Magic));
      writer.Write(events.Count);
      foreach (var evt in events) {
        writer.Write(evt.ColumnMask);
        writer.Write(evt.ColumnMaskHigh);
        for (var column = 0; column < ColumnCount; column++) {
          if (!evt.ColumnIsSet(column)) continue;
          WriteValue(writer, ProfilerEventColumns.ProfilerColumnDataTypes[column], evt.Values[column]);
        }
      }
    }

    public static List<ProfilerEvent> Load(string path) {
      try {
        using var file = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var gzip = new GZipStream(file, CompressionMode.Decompress);
        using var reader = new BinaryReader(gzip, Encoding.UTF8);
        if (Encoding.ASCII.GetString(reader.ReadBytes(Magic.Length)) != Magic)
          throw new InvalidDataException("This is not a SqlProfiler trace file.");

        var count = reader.ReadInt32();
        if (count < 0) throw new InvalidDataException("The trace file is damaged.");
        var events = new List<ProfilerEvent>(Math.Min(count, 100000));
        for (var i = 0; i < count; i++) {
          var evt = new ProfilerEvent();
          evt.ColumnMask = reader.ReadUInt64();
          evt.ColumnMaskHigh = reader.ReadUInt64();
          for (var column = 0; column < ColumnCount; column++) {
            if (!evt.ColumnIsSet(column)) continue;
            evt.Values[column] = ReadValue(reader, ProfilerEventColumns.ProfilerColumnDataTypes[column]);
          }

          events.Add(evt);
        }

        return events;
      }
      catch (EndOfStreamException e) {
        throw new InvalidDataException("The trace file is damaged.", e);
      }
    }

    private static void WriteValue(BinaryWriter writer, ProfilerColumnDataType type, object value) {
      switch (type) {
        case ProfilerColumnDataType.Int:
          writer.Write((int) value);
          break;
        case ProfilerColumnDataType.Long:
          writer.Write((long) value);
          break;
        case ProfilerColumnDataType.String:
          writer.Write((string) value ?? "");
          break;
        case ProfilerColumnDataType.DateTime:
          writer.Write(((DateTime) value).Ticks);
          break;
        case ProfilerColumnDataType.Byte: {
          var bytes = (byte[]) value ?? Array.Empty<byte>();
          writer.Write(bytes.Length);
          writer.Write(bytes);
          break;
        }
        case ProfilerColumnDataType.Guid:
          writer.Write(((Guid) value).ToByteArray());
          break;
      }
    }

    private static object ReadValue(BinaryReader reader, ProfilerColumnDataType type) {
      switch (type) {
        case ProfilerColumnDataType.Int: return reader.ReadInt32();
        case ProfilerColumnDataType.Long: return reader.ReadInt64();
        case ProfilerColumnDataType.String: return reader.ReadString();
        case ProfilerColumnDataType.DateTime: return new DateTime(reader.ReadInt64());
        case ProfilerColumnDataType.Byte: {
          var length = reader.ReadInt32();
          if (length < 0 || length > 64 * 1024 * 1024) throw new InvalidDataException("The trace file is damaged.");
          return reader.ReadBytes(length);
        }
        case ProfilerColumnDataType.Guid: return new Guid(reader.ReadBytes(16));
        default: return null;
      }
    }
  }
}
