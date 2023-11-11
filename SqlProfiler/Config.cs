using System;
using System.IO;
using System.Windows.Forms;

namespace SqlProfiler {

  internal class Config {
      
    private static readonly Lazy<Config> instanceField = new Lazy<Config>(Load);
    internal static Config Instance => instanceField.Value;
      
    public Config() {
        
      Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
      Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
      Left = Screen.PrimaryScreen.WorkingArea.Left + Width / 2;
      Top = Screen.PrimaryScreen.WorkingArea.Top + Height / 2;
        
      User = "sa";
      Password = "sa";
      Server = ".";
    }

    public int Left { get; set; }
    public int Top { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }

    public string Server { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
      
    private static string GetSettingsFilePath() {
      return Path.ChangeExtension(Updater.CurrentFileLocation, ".json");
    }

    internal void Save() {
      var fileName = GetSettingsFilePath();
      GC.Collect(GC.MaxGeneration);
      File.WriteAllText(fileName, this.ToJson());
    }

    private static Config Load() {
      var fileName = GetSettingsFilePath();
      try {
        if (File.Exists(fileName)) {
          var fileData = File.ReadAllText(fileName);
          var result = fileData.FromJson<Config>();
          if (result != null)
            return result;
        }
      }
      catch {
        // ignored
      }

      return new Config();
    }
  }
}