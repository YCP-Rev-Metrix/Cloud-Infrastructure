using Common.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DatabaseCore.DatabaseComponents;

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
            ? $"Server=143.110.146.58,1433;User Id=SA;Password=BigPass@Word!;TrustServerCertificate=True;"
            // $"Data Source=localhost;Integrated Security=True;TrustServerCertificate=True;" // Need to come back to this to fix local development 
            : DockerizedEnviron == "Dockerized"
                ? $"Server=143.110.146.58,1433;User Id=SA;Password=BigPass@Word!;TrustServerCertificate=True;"
                : $"Data Source=localhost;Integrated Security=True;TrustServerCertificate=True;";

        //LogWriter.LogInfo($"Server=143.110.146.58,1433;User Id=SA;Password=BigPass@Word!;TrustServerCertificate=True;");








        /*********************************************************************************************************
         * THIS WILL BE USED FOR SECURITY, NEED TO FIX PROCESSING OF ENV VARAIBLES LOCALLY BEFORE COMMENTING OUT *
         *********************************************************************************************************/

        /*// Get DOCKERIZED and Connection String environment variables
        string? DockerizedEnviron = Environment.GetEnvironmentVariable("DOCKERIZED");
        string? ServerConnectionString = "" + Environment.GetEnvironmentVariable("SERVER_CONNECTION_STRING");
        string? LocalConnectionString = "" + Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING");

        ConnectionString = DockerizedEnviron == null
            ? $"{ServerConnectionString}" // Force cloud connection string since local development does not work at the moment
            // LocalConnectionString // uncomment this local connection string once local dev works again 
            : DockerizedEnviron == "Dockerized"
                ? $"{ServerConnectionString}"
                : $"{LocalConnectionString}";

        LogWriter.LogInfo($"DB Connection: {ConnectionString}");
        LogWriter.LogInfo($"Server String: {ServerConnectionString}");
        LogWriter.LogInfo($"Local String: {LocalConnectionString}");*/


        DatabaseName = databaseName;
        Initialize();

    }

    public void Initialize()
    {
        /*
        serverConnection = new ServerConnection("localhost");
        Server = new Server(serverConnection);
        */

        Server = new Server(new ServerConnection(new SqlConnection(ConnectionString)));
        database = Server.Databases[DatabaseName];
    }
}
