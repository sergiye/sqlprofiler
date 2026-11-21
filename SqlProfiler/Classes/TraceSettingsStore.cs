using sergiye.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SqlProfiler {
  /// <summary>Trace settings (events, columns, filters) kept as text in the application settings, so they follow the portable/registry choice.</summary>
  internal static class TraceSettingsXml {
    private static readonly XmlSerializer Serializer = new(typeof(TraceProperties.TraceSettings));

    public static string Serialize(TraceProperties.TraceSettings settings) {
      using var writer = new StringWriter();
      Serializer.Serialize(writer, settings);
      return writer.ToString();
    }

    public static TraceProperties.TraceSettings TryDeserialize(string xml) {
      if (string.IsNullOrEmpty(xml)) return null;
      try {
        using var reader = new StringReader(xml);
        return Serializer.Deserialize(reader) as TraceProperties.TraceSettings;
      }
      catch (Exception e) {
        AppLogger.Warn("Stored trace settings are ignored: " + e.Message);
        return null;
      }
    }
  }

  /// <summary>Named sets of trace settings.</summary>
  internal sealed class PresetStore {
    private const string NamesKey = "PresetNames";
    private const string PresetPrefix = "Preset_";
    private const char Separator = '|';
    private const int MaxNameLength = 50;

    private readonly PersistentSettings _settings;

    public PresetStore(PersistentSettings settings) {
      _settings = settings;
    }

    public static string NormalizeName(string name) {
      name = (name ?? "").Replace(Separator, '-').Trim();
      return name.Length > MaxNameLength ? name.Substring(0, MaxNameLength).Trim() : name;
    }

    public List<string> Names {
      get {
        return _settings.GetValue(NamesKey, "").Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries)
          .Select(n => n.Trim())
          .Where(n => n.Length > 0)
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
          .ToList();
      }
    }

    public bool Exists(string name) {
      return Names.Any(n => string.Equals(n, NormalizeName(name), StringComparison.OrdinalIgnoreCase));
    }

    public TraceProperties.TraceSettings Load(string name) {
      var existing = Names.FirstOrDefault(n => string.Equals(n, NormalizeName(name), StringComparison.OrdinalIgnoreCase));
      return existing is null ? null : TraceSettingsXml.TryDeserialize(_settings.GetValue(PresetPrefix + existing, ""));
    }

    public void Save(string name, TraceProperties.TraceSettings settings) {
      name = NormalizeName(name);
      if (name.Length == 0) throw new ArgumentException("The name of the preset is empty.");
      var names = Names;
      var existing = names.FirstOrDefault(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
      if (existing is not null) name = existing;
      else names.Add(name);
      _settings.SetValue(PresetPrefix + name, TraceSettingsXml.Serialize(settings));
      _settings.SetValue(NamesKey, string.Join(Separator.ToString(), names));
    }

    public void Delete(string name) {
      var existing = Names.FirstOrDefault(n => string.Equals(n, NormalizeName(name), StringComparison.OrdinalIgnoreCase));
      if (existing is null) return;
      _settings.Remove(PresetPrefix + existing);
      _settings.SetValue(NamesKey, string.Join(Separator.ToString(), Names.Where(n => n != existing)));
    }
  }
}
