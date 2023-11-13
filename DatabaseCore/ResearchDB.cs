using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace DatabaseCore;

public class ResearchDB : AbstractDatabase
{
    public ResearchDB() : base("revmetrix-r")
    {

    }

    public async Task Kill()
    {
        // Database?.DropIfExists();
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        string noConstraint = "Use [revmetrix-r] ALTER TABLE [Shot] NOCHECK CONSTRAINT all";
        using var command = new SqlCommand(noConstraint, connection);
        command.ExecuteNonQuery();

        string dropShot = "DROP TABLE [Shot]";
        using var command2 = new SqlCommand(dropShot, connection);
        command2.ExecuteNonQuery();

    }
    public void CreateTables()
    {
        database = new Microsoft.SqlServer.Management.Smo.Database(Server, DatabaseName);
        if (!Server.Databases.Contains(DatabaseName))
        {
            database.Create();
        }
        // Otherwise breakout of createTables
        else
        {
            return;
        }

        //Shot table 
        {
            var ShotTable = new Table(database, "Shot");

            var pins_remaining = new Column(ShotTable, "pins_remaining", DataType.Binary(2)) //4 bits for each Binary
            {
                Nullable = false
            };
            ShotTable.Columns.Add(pins_remaining);

            var time = new Column(ShotTable, "time", DataType.DateTime)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(time);

            var lane_number = new Column(ShotTable, "lane_Number", DataType.Binary(2))
            {
                Nullable = false
            };
            ShotTable.Columns.Add(lane_number);

            //Index x = new Index(Database, "xIndex");

            var x_postions = new Column(ShotTable, "x_positions", DataType.Float)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(x_postions);

            var y_postions = new Column(ShotTable, "y_positions", DataType.Float)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(y_postions);

            var z_postions = new Column(ShotTable, "z_positions", DataType.Float)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(z_postions);

            //ShotTable.Columns[0].Create();

            // ID
            var id = new Column(ShotTable, "id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            ShotTable.Columns.Add(id);

            ShotTable.Create();

            // Create the primary key constraint using SQL
            string sql = "ALTER TABLE [Shot] ADD CONSTRAINT PK_User PRIMARY KEY (id);";
            database.ExecuteNonQuery(sql);

            // TODO: add shot_id PK as a SQL Query
            // TODO: add ball_id FK as a SQL Query
            // TODO: add video_id FK as a SQL Query

        }

    }

    public async Task<bool> AddShot(byte[] pins_remaining, DateTime time, byte[] lane_number, float x_positions, float y_positions, float z_positions)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [Shot] (pins_remaining, time, lane_number, x_positions, y_positions, z_positions) " +
                             "VALUES (@Pins_remaining, @Time, @Lane_number, @x_positions, @y_positions, @z_positions)";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@Pins_remaining", SqlDbType.Binary).Value = pins_remaining;
        command.Parameters.Add("@Time", SqlDbType.DateTime).Value = time;
        command.Parameters.Add("@Lane_number", SqlDbType.Binary).Value = lane_number;
        command.Parameters.Add("@x_positions", SqlDbType.Float).Value = x_positions;
        command.Parameters.Add("@y_positions", SqlDbType.Float).Value = y_positions;
        command.Parameters.Add("@z_positions", SqlDbType.Float).Value = z_positions;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public async Task<(bool success, DateTime time)> GetShotData(byte[] lane_number)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string selectQuery = "SELECT time, x_positions, y_positions, z_positions FROM [Shot] WHERE Lane_number = @Lane_number";

        using var command = new SqlCommand(selectQuery, connection);
        command.Parameters.Add("@Lane_number", SqlDbType.VarChar, 255).Value = lane_number;

        using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            // Retrieve the columns

            var db_time = Convert.ToDateTime(reader["time"].ToString());

            return (true, db_time);
        }
        else
        {
            return (false, DateTime.UnixEpoch);
        }
    }

}
