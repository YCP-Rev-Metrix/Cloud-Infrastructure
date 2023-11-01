using Common.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DatabaseCore;

public abstract class AbstractDatabase
{
    protected Server Server;
    public Database Database;
    public string DatabaseName { get; set; }
    public string ConnectionString { get; set; }

    public AbstractDatabase(string databaseName)
    {
        // Get DOCKERIZED environment variable
        string? DockerizedEnviron = Environment.GetEnvironmentVariable("DOCKERIZED");
        ConnectionString = DockerizedEnviron == null
            ? $"Data Source=localhost;database={databaseName};Integrated Security=True;TrustServerCertificate=True;"
            : DockerizedEnviron == "Dockerized"
                ? $"Server=sql_server;database={databaseName};User Id=SA;Password=BigPass@Word!;TrustServerCertificate=True;"
                : $"Data Source=localhost;database={databaseName};Integrated Security=True;TrustServerCertificate=True;";

        LogWriter.LogInfo($"DB Connection: {ConnectionString}");

        DatabaseName = databaseName;
        Initialize();
    }

    public void Initialize()
    {
        Server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
        Database = Server.Databases[DatabaseName];
    }

}
