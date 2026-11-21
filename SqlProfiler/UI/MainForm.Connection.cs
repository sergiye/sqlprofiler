using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlProfiler {
  public partial class MainForm {
    private const string RecentServersSetting = "RecentServers";

    private ToolStripDropDownButton _ddRecentServers;
    private ToolStripButton _tbTestConnection;

    /// <summary>Recent servers (a drop-down list and auto complete) and the "Test connection" button.</summary>
    private void InitConnectionTools() {
      _ddRecentServers = new ToolStripDropDownButton("▾") {ToolTipText = "Recent servers", ShowDropDownArrow = false};
      _ddRecentServers.DropDownOpening += (_, _) => FillRecentServersMenu();
      toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(edServer) + 1, _ddRecentServers);

      _tbTestConnection = new ToolStripButton("Test") {ToolTipText = "Test the connection and the ALTER TRACE permission"};
      _tbTestConnection.Click += async (_, _) => await TestConnectionAsync();
      toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(toolStripSeparator3), _tbTestConnection);

      var textBox = edServer.TextBox;
      textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
      RefreshServerAutoComplete();
    }

    private void FillRecentServersMenu() {
      _ddRecentServers.DropDownItems.Clear();
      var servers = RecentServers.Parse(_settings.GetValue(RecentServersSetting, ""));
      foreach (var server in servers) {
        var name = server;
        _ddRecentServers.DropDownItems.Add(new ToolStripMenuItem(name, null, (_, _) => edServer.Text = name));
      }

      if (servers.Count == 0)
        _ddRecentServers.DropDownItems.Add(new ToolStripMenuItem("No recent servers") {Enabled = false});
    }

    private void RefreshServerAutoComplete() {
      var source = new AutoCompleteStringCollection();
      source.AddRange(RecentServers.Parse(_settings.GetValue(RecentServersSetting, "")).ToArray());
      edServer.TextBox.AutoCompleteCustomSource = source;
    }

    private void RememberServer(string server) {
      _settings.SetValue(RecentServersSetting, RecentServers.Add(_settings.GetValue(RecentServersSetting, ""), server));
      RefreshServerAutoComplete();
    }

    private async Task TestConnectionAsync() {
      _tbTestConnection.Enabled = false;
      Cursor = Cursors.WaitCursor;
      var server = edServer.Text;
      try {
        var connection = CreateConnection();
        var info = await Task.Run(() => ReadServerInfo(connection));
        RememberServer(server);
        MessageBox.Show($"Connected to {server}\r\n\r\n{info.Version}\r\n\r\nALTER TRACE permission: {(info.CanTrace ? "yes" : "NO - the trace cannot be started with this login")}",
          "Test connection", MessageBoxButtons.OK, info.CanTrace ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
      }
      catch (Exception e) {
        AppLogger.Error("Test connection failed", e);
        MessageBox.Show("Cannot connect to " + server + ":\r\n" + e.Message, "Test connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        Cursor = Cursors.Default;
        _tbTestConnection.Enabled = true;
      }
    }

    private static (string Version, bool CanTrace) ReadServerInfo(SqlConnection connection) {
      using (connection) {
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT @@VERSION, ISNULL(HAS_PERMS_BY_NAME(NULL, NULL, 'ALTER TRACE'), 0)";
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return ("", false);
        var version = reader.GetString(0).Split('\n')[0].Trim();
        return (version, Convert.ToInt32(reader.GetValue(1)) == 1);
      }
    }
  }
}
