using Common.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DatabaseCore;

public abstract class AbstractDatabase
{
    protected Server Server;
    public Database database;
    public string DatabaseName { get; set; }
    public string ConnectionString { get; set; }

    public AbstractDatabase(string databaseName)
    {
        // Get DOCKERIZED environment variable
        string? DockerizedEnviron = Environment.GetEnvironmentVariable("DOCKERIZED");
        ConnectionString = DockerizedEnviron == null
            ? $"Data Source=localhost;Integrated Security=True;TrustServerCertificate=True;"
            : DockerizedEnviron == "Dockerized"
                ? $"Server=143.110.146.58,1433;User Id=SA;Password=BigPass@Word!;TrustServerCertificate=True;"
                : $"Data Source=localhost;Integrated Security=True;TrustServerCertificate=True;";

        LogWriter.LogInfo($"DB Connection: {ConnectionString}");

        DatabaseName = databaseName;
        Initialize();
    }

    public void Initialize()
    {
        // serverConnection = new ServerConnection("localhost");
        // Server = new Server(serverConnection);
        Server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
        database = Server.Databases[DatabaseName];



    }

}
