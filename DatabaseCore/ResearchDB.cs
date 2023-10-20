using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace Database
{
    public class ResearchDB : AbstractDatabase
    {
        public ResearchDB() : base("revmetrix-r")
        {

        }

        public void CreateTables()
        {
            Database = new Microsoft.SqlServer.Management.Smo.Database(Server, DatabaseName);
            Database.Create();

            //Shot table 
            {
                var ShotTable = new Table(Database, "Shot");

                var pins_remaining = new Column(Database, "Pins_Remaining", DataType.Binary(2)) //4 bits for each Binary
                {
                    Nullable = false
                };
                ShotTable.Columns.Add(pins_remaining);


                var time = new Column(Database, "Time", DataType.DateTime)
                {
                    Nullable = false
                };
                ShotTable.Columns.Add(time);


                var lane_number = new Column(Database, "Lane_Number", DataType.Binary(2))
                {
                    Nullable = false
                };
                ShotTable.Columns.Add(lane_number);
                
                //Index x = new Index(Database, "xIndex");

                var x_postions = new Column(Database, "x_postions", DataType.Float)
                {
                    Nullable = false
                };
                
                var y_postions = new Column(Database, "y_postions", DataType.Float)
                {
                    Nullable = false
                };
                ShotTable.Columns.Add(y_postions);
                
                
                var z_postions = new Column(Database, "z_postions", DataType.Float)
                {
                    Nullable = false
                };
                ShotTable.Columns.Add(z_postions);

                ShotTable.Create();



                ShotTable.Columns[0].Create();

                // TODO: add shot_id PK as a SQL Query
                // TODO: add ball_id FK as a SQL Query
                // TODO: add video_id FK as a SQL Query


            }

        }

        public async Task<bool> AddShot(byte[] pins_remaining, DateTime time, byte[] lane_number, float x_postions, float y_postions, float z_postions)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string insertQuery = "INSERT INTO [Shot] (pins_remaining, time, lane_number, x_postions, y_postions, z_postions) " +
                                     "VALUES (@Pins_remaining, @Time, @Lane_number, @x_postions, @y_postions, @z_postions)";

                using (var command = new SqlCommand(insertQuery, connection))
                {
                    // Set the parameter values
                    command.Parameters.Add("@Pins_remaining", SqlDbType.Binary).Value = pins_remaining;
                    command.Parameters.Add("@Time", SqlDbType.DateTime).Value = time;
                    command.Parameters.Add("@Lane_number", SqlDbType.Binary).Value = lane_number;
                    command.Parameters.Add("@x_postions", SqlDbType.Float).Value = x_postions;
                    command.Parameters.Add("@y_postions", SqlDbType.Float).Value = y_postions;
                    command.Parameters.Add("@z_postions", SqlDbType.Float).Value = z_postions;

                    // Execute the query
                    int i = await command.ExecuteNonQueryAsync();
                    return i != -1;
                }
            }
        }







    }

}
