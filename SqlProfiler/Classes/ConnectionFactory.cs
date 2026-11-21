using System.Data.SqlClient;

namespace SqlProfiler {
  internal sealed class ConnectionOptions {
    public string Server { get; set; }
    public bool IntegratedSecurity { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public bool Encrypt { get; set; }
    public bool TrustServerCertificate { get; set; }
  }

  internal static class ConnectionFactory {
    public static SqlConnection Create(ConnectionOptions options) {
      var builder = new SqlConnectionStringBuilder {
        DataSource = options.Server,
        InitialCatalog = "master",
        ApplicationName = "SqlProfiler",
        ConnectTimeout = 15,
        Encrypt = options.Encrypt,
        TrustServerCertificate = options.TrustServerCertificate
      };
      if (options.IntegratedSecurity) {
        builder.IntegratedSecurity = true;
      }
      else {
        builder.UserID = options.User;
        builder.Password = options.Password;
      }

      return new SqlConnection(builder.ConnectionString);
    }
  }
}
