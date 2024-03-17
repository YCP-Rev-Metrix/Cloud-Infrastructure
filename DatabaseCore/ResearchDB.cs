using System.Collections;
using Microsoft.Data.SqlClient;
using Common.POCOs;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Reflection.Metadata;
using Microsoft.SqlServer.Management.Common;


namespace DatabaseCore;

public class ResearchDB : AbstractDatabase
{
    public ResearchDB() : base("revmetrix-r")
    {
        
    }


    public void CreateTables()
    {

        ServerConnection serverConnection = new ServerConnection("localhost");
        Server server = new Server(serverConnection);
        var database = new Microsoft.SqlServer.Management.Smo.Database(Server, DatabaseName);

        // Will need to look at this part!
        if (!Server.Databases.Contains(DatabaseName))
        {
            database.Create();
        }
        else
        {

        }
        // Using the database this way due to some error that happens when using database above with the new object
        // Continues to give error about not being able to create the tables when parent isn't created first
        // Even tho the parent is created. 
        Database temp = Server.Databases[DatabaseName];

        //Shot table 
        {
            var ShotTable = new Table(temp, "Shot");

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



            if (!temp.Tables.Contains("Shot"))
            {
                ShotTable.Create();
                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Shot] ADD CONSTRAINT PK_User PRIMARY KEY (id);";
                
                temp.ExecuteNonQuery(sql);

                // Create the foreign key after the "ShotTable" has been created
                {
                    ShotTable = temp.Tables["Shot"]; // Retrieve the existing "ShotTable"

                    // ball id                             Foriegn Key from Ball
                    var ballIdKey = new ForeignKey(ShotTable, "FK_Shot_Ball");
                    var ballIdKeyCol = new ForeignKeyColumn(ballIdKey, "Ball_id")
                    {
                        ReferencedColumn = "Ball_id"
                    };
                    ballIdKey.Columns.Add(ballIdKeyCol);
                    ballIdKey.ReferencedTable = "Ball";

                    ballIdKey.Create();

                    // video id                             Foriegn Key from Video
                    var videoIdKey = new ForeignKey(ShotTable, "FK_Shot_Video");
                    var videoIdKeyCol = new ForeignKeyColumn(ballIdKey, "Video_id")
                    {
                        ReferencedColumn = "video_id"
                    };
                    videoIdKey.Columns.Add(videoIdKeyCol);
                    videoIdKey.ReferencedTable = "Video";

                    videoIdKey.Create();

                }

            }
            
            



        }

        //Video table
        {
            var VideoTable = new Table(temp, "Video");
            

            var video = new Column(VideoTable, "video", DataType.VarBinaryMax) //SQLserver does not support blob ... blob is just a large binary file 
            {
                Nullable = false
            };
            VideoTable.Columns.Add(video);


            // ID
            var video_id = new Column(VideoTable, "id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            VideoTable.Columns.Add(video_id);



            if (!temp.Tables.Contains("Video"))
            {
                VideoTable.Create();
                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Video] ADD CONSTRAINT PK_User PRIMARY KEY (id);";
                
                temp.ExecuteNonQuery(sql);

                //Not Sure if this will apply 
                sql = "ALTER TABLE [Video] ADD CONSTRAINT UNQ__User__video UNIQUE ([video])";
                temp.ExecuteNonQuery(sql);

            }


        }

        //Ball table
        {
            var BallTable = new Table(temp, "Ball");

            var weight = new Column(BallTable, "weight", DataType.Float ) //4 bits for each Binary
            {
                Nullable = false
            };
            BallTable.Columns.Add(weight);

            var color = new Column(BallTable, "color", DataType.VarChar(50))
            {
                Nullable = false
            };
            BallTable.Columns.Add(color);


            // ID
            var id = new Column(BallTable, "id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            BallTable.Columns.Add(id);


            if (!temp.Tables.Contains("Ball"))
            {
                BallTable.Create();
                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Ball] ADD CONSTRAINT PK_User PRIMARY KEY (id);";
                
                temp.ExecuteNonQuery(sql);

                //Not Sure if this will apply this might change !!! 
                sql = "ALTER TABLE [Ball] ADD CONSTRAINT UNQ__User__Ball UNIQUE ([Ball])";
                temp.ExecuteNonQuery(sql);


            }

           


        }

        //ddx table
        {
            var ddxTable = new Table(temp, "ddx");

            var x_accelerations = new Column(ddxTable, "x_accelerations", DataType.Float ) //4 bits for each Binary
            {
                Nullable = false
            };
            ddxTable.Columns.Add(x_accelerations);

            // TODO: ball_id as FK

            if (!temp.Tables.Contains("ddx"))
            {
                ddxTable.Create();
                
                {
                    // Ball id                             Foriegn Key from Event
                    var ballIdKey = new ForeignKey(ddxTable, "FK_ddx_Ball");
                    var ballIdKeyCol = new ForeignKeyColumn(ballIdKey, "id")
                    {
                        ReferencedColumn = "id"
                    };
                    ballIdKey.Columns.Add(ballIdKeyCol);
                    ballIdKey.ReferencedTable = "Ball";

                    ballIdKey.Create();

                }
            }
            
        }

        //ddy table
        {
            var ddyTable = new Table(temp, "ddy");

            var y_accelerations = new Column(ddyTable, "y_accelerations", DataType.Float ) //4 bits for each Binary
            {
                Nullable = false
            };
            ddyTable.Columns.Add(y_accelerations);

            // TODO: ball_id as FK

            if (!temp.Tables.Contains("ddy"))
            {
                ddyTable.Create();

                {
                    // Ball id                             Foriegn Key from Event
                    var ballIdKey = new ForeignKey(ddyTable, "FK_ddy_Ball");
                    var ballIdKeyCol = new ForeignKeyColumn(ballIdKey, "id")
                    {
                        ReferencedColumn = "id"
                    };
                    ballIdKey.Columns.Add(ballIdKeyCol);
                    ballIdKey.ReferencedTable = "Ball";

                    ballIdKey.Create();

                }

            }
        }

        //ddz table
        {
            var ddzTable = new Table(temp, "ddz");

            var z_accelerations = new Column(ddzTable, "z_accelerations", DataType.Float ) //4 bits for each Binary
            {
                Nullable = false
            };
            ddzTable.Columns.Add(z_accelerations);

            // TODO: ball_id as FK

            if (!temp.Tables.Contains("ddz"))
            {
                ddzTable.Create();

                {
                    // Ball id                             Foriegn Key from Event
                    var ballIdKey = new ForeignKey(ddzTable, "FK_ddz_Ball");
                    var ballIdKeyCol = new ForeignKeyColumn(ballIdKey, "id")
                    {
                        ReferencedColumn = "id"
                    };
                    ballIdKey.Columns.Add(ballIdKeyCol);
                    ballIdKey.ReferencedTable = "Ball";

                    ballIdKey.Create();

                }

            }
        }

        //Light table
        {
            var LightTable = new Table(temp, "light");
            

            Column[] Light_levels = new Column[3];

            for ( int i = 0; i < Light_levels.Length; i++)
            {
                Light_levels[i] = new Column(LightTable, "light_levels", DataType.Float ) //4 bits for each Binary
                {
                    Nullable = false
                };
                LightTable.Columns.Add(Light_levels[i]);
            }
            
            
            // TODO: ball_id as FK

            if (!temp.Tables.Contains("light"))
            {
                LightTable.Create();

                {
                    // Ball id                             Foriegn Key from Event
                    var ballIdKey = new ForeignKey(LightTable, "FK_light_Ball");
                    var ballIdKeyCol = new ForeignKeyColumn(ballIdKey, "id")
                    {
                        ReferencedColumn = "id"
                    };
                    ballIdKey.Columns.Add(ballIdKeyCol);
                    ballIdKey.ReferencedTable = "Ball";

                    ballIdKey.Create();

                }

            }
        }

    }

    public async Task Kill()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string dropShot = "DROP TABLE [Shot]";
        using var command = new SqlCommand(dropShot, connection);
        _ = command.ExecuteNonQuery();

        string dropVideo = "DROP TABLE [Video]";
        using var command2 = new SqlCommand(dropVideo, connection);
        _ = command2.ExecuteNonQuery();

        string dropBall = "DROP TABLE [Ball]";
        using var command3 = new SqlCommand(dropBall, connection);
        _ = command3.ExecuteNonQuery();

        string drop_ddx = "DROP TABLE [ddx]";
        using var command4 = new SqlCommand(drop_ddx, connection);
        _ = command4.ExecuteNonQuery();

        string drop_ddy = "DROP TABLE [ddy]";
        using var command5 = new SqlCommand(drop_ddy, connection);
        _ = command5.ExecuteNonQuery();

        string drop_ddz= "DROP TABLE [ddz]";
        using var command6 = new SqlCommand(drop_ddz, connection);
        _ = command6.ExecuteNonQuery();

        string drop_light= "DROP TABLE [Light]";
        using var command7 = new SqlCommand(drop_light, connection);
        _ = command7.ExecuteNonQuery();

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

    public async Task<bool> AddVideo(float video, int video_id)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [Video] (video, id) " +
                             "VALUES (@video, @id)";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@video", SqlDbType.VarBinary).Value = video;
        command.Parameters.Add("@id", SqlDbType.BigInt).Value = video_id;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public async Task<bool> AddDdx(float x_accelerations)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [Ddx] (x_accelerations) " +
                             "VALUES (@x_accelerations)";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@x_accelerations", SqlDbType.Float).Value = x_accelerations;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public async Task<bool> AddDdy(float y_accelerations)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [Ddy] (y_accelerations) " +
                             "VALUES (@y_accelerations)";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@y_accelerations", SqlDbType.Float).Value = y_accelerations;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public async Task<bool> AddDdz(float z_accelerations)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [Ddz] (z_accelerations) " +
                             "VALUES (@z_accelerations)";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@z_accelerations", SqlDbType.Float).Value = z_accelerations;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public async Task<bool> AddLight(float[] light_levels)
    {
        int i = 0;
        
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();

            for (int j = 0; j < 3; j++)
            {
            
                string insertQuery = "INSERT INTO [Light] (light_levels) " +
                             "VALUES (@light_levels)";

                using (var command = new SqlCommand(insertQuery, connection))
                {

                    // Set the parameter values
                    command.Parameters.Add("@light_levels", SqlDbType.Float).Value = light_levels;
                    
                    // Execute the query
                    i = await command.ExecuteNonQueryAsync();
                    
                }
            }
        }

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

    public async Task<bool> AddUserDatabase()
    {
        Database userDB = Server.Databases["revmetrix-u"];

        
        
        return false;
    }

}
