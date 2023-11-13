using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using Microsoft.SqlServer.Management.Common;
using System.Xml;

namespace DatabaseCore;

public class UserDB : AbstractDatabase
{
    public UserDB() : base("revmetrix-u")
    {

    }

    public void CreateTables()
    {

        ServerConnection serverConnection = new ServerConnection("localhost");
        Server server = new Server(serverConnection);
        Database database = new Microsoft.SqlServer.Management.Smo.Database(Server, DatabaseName);
        
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

        // User Table
        {
            // Create new
            var UserTable = new Table(temp, "User");

            // Firstname
            var firstname = new Column(UserTable, "firstname", DataType.VarChar(255))
            {
                Nullable = false
            };
            UserTable.Columns.Add(firstname);

            // Lastname
            var lastname = new Column(UserTable, "lastname", DataType.VarChar(255))
            {
                Nullable = false
            };
            UserTable.Columns.Add(lastname);

            // Username
            var username = new Column(UserTable, "username", DataType.VarChar(255))
            {
                Nullable = false
            };
            UserTable.Columns.Add(username);

            // Salt
            var salt = new Column(UserTable, "salt", DataType.VarBinary(16))
            {
                Nullable = false
            };
            UserTable.Columns.Add(salt);

            // Roles
            var roles = new Column(UserTable, "roles", DataType.VarChar(255))
            {
                Nullable = false
            };
            UserTable.Columns.Add(roles);

            // Password
            var password = new Column(UserTable, "password", DataType.VarBinaryMax)
            {
                Nullable = false
            };
            UserTable.Columns.Add(password);

            // Email
            var email = new Column(UserTable, "email", DataType.VarChar(255));
            UserTable.Columns.Add(email);

            // Phone
            var phone = new Column(UserTable, "phone", DataType.VarChar(255));
            UserTable.Columns.Add(phone);

            // ID
        
            var id = new Column(UserTable, "id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            UserTable.Columns.Add(id);


            if (!temp.Tables.Contains("User"))
            {
                UserTable.Create();


                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [User] ADD CONSTRAINT PK_User PRIMARY KEY (id);";

                temp.ExecuteNonQuery(sql);

                sql = "ALTER TABLE [User] ADD CONSTRAINT UNQ__User__username UNIQUE ([username])";
                temp.ExecuteNonQuery(sql);
            }
        }

        // RefreshToken Table
        {
            // Create new
            var TokenTable = new Table(temp, "RefreshToken");

            // Expiration
            var expiration = new Column(TokenTable, "expiration", DataType.DateTime)
            {
                Nullable = false
            };
            TokenTable.Columns.Add(expiration);

            // User ID
            var userId = new Column(TokenTable, "userid", DataType.BigInt)
            {
                Nullable = false
            };
            TokenTable.Columns.Add(userId);

            // Token
            var token = new Column(TokenTable, "token", DataType.VarBinary(32))
            {
                Nullable = false
            };
            TokenTable.Columns.Add(token);


            if (!temp.Tables.Contains("RefreshToken"))
            {
                TokenTable.Create();

                // Create the foreign key after the "RefreshTokenTable" has been created
                {
                    TokenTable = temp.Tables["RefreshToken"]; // Retrieve the existing "RefreshTokenTable"

                    // User ID
                    var userIdKey = new ForeignKey(TokenTable, "FK_RefreshToken_User");
                    var userIdKeyCol = new ForeignKeyColumn(userIdKey, "userid")
                    {
                        ReferencedColumn = "id"
                    };
                    userIdKey.Columns.Add(userIdKeyCol);
                    userIdKey.ReferencedTable = "User";

                    userIdKey.Create();
                }
            }
        }

        // Event Table
        {
            // Create new Table
            var EventTable = new Table(temp, "Event");

            // Game id
            var event_id = new Column(EventTable, "event_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            EventTable.Columns.Add(event_id);

            // User ID
            var userId = new Column(EventTable, "userid", DataType.BigInt)
            {
                Nullable = false
            };
            EventTable.Columns.Add(userId);

            // Event Type
            var event_type = new Column(EventTable, "event_type", DataType.VarChar(50));
            EventTable.Columns.Add(event_type);


            if (!temp.Tables.Contains("Event"))
            {
                EventTable.Create();

                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Event] ADD CONSTRAINT PK_Event PRIMARY KEY (event_id);";
                temp.ExecuteNonQuery(sql);

                // Create the foreign key after the "EventTable" has been created
                {
                    EventTable = temp.Tables["Event"]; // Retrieve the existing "EventTable"

                    // User ID
                    var userIdKey = new ForeignKey(EventTable, "FK_Event_User");
                    var userIdKeyCol = new ForeignKeyColumn(userIdKey, "userid")
                    {
                        ReferencedColumn = "id"
                    };
                    userIdKey.Columns.Add(userIdKeyCol);
                    userIdKey.ReferencedTable = "User";

                    userIdKey.Create();
                }
            }


        }

        // League Table
        {
            // Create new table
            var LeagueTable = new Table(temp, "League");

            // league_id
            var league_id = new Column(LeagueTable, "league_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            LeagueTable.Columns.Add(league_id);

            // Event Id
            var event_id = new Column(LeagueTable, "event_id", DataType.BigInt)
            {
                Nullable = false
            };
            LeagueTable.Columns.Add(event_id);

            if (!temp.Tables.Contains("League"))
            {
                LeagueTable.Create();

                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [League] ADD CONSTRAINT PK_League PRIMARY KEY (league_id);";
                temp.ExecuteNonQuery(sql);

                // Create the foreign key after the "LeagueTable" has been created
                {
                    LeagueTable = temp.Tables["League"]; // Retrieve the existing "LeagueTable"

                    // Event id                             Foriegn Key from Event
                    var eventIdKey = new ForeignKey(LeagueTable, "FK_League_Event");
                    var eventIdKeyCol = new ForeignKeyColumn(eventIdKey, "event_id")
                    {
                        ReferencedColumn = "event_id"
                    };
                    eventIdKey.Columns.Add(eventIdKeyCol);
                    eventIdKey.ReferencedTable = "Event";

                    eventIdKey.Create();
                }
            }
        }

        // Tournament Table
        {
            // Create new table
            var TournamentTable = new Table(temp, "Tournament");

            // tournament_id
            var tournament_id = new Column(TournamentTable, "tournament_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            TournamentTable.Columns.Add(tournament_id);

            // Event Id
            var event_id = new Column(TournamentTable, "event_id", DataType.BigInt)
            {
                Nullable = false
            };
            TournamentTable.Columns.Add(event_id);

            if (!temp.Tables.Contains("Tournament"))
            {
                TournamentTable.Create();

                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Tournament] ADD CONSTRAINT PK_Tournament PRIMARY KEY (tournament_id);";
                temp.ExecuteNonQuery(sql);

                // Create the foreign key after the "TournamentTable" has been created
                {
                    TournamentTable = temp.Tables["Tournament"]; // Retrieve the existing "PracticeTable"

                    // Event id                             Foriegn Key from Event
                    var eventIdKey = new ForeignKey(TournamentTable, "FK_Tournament_Event");
                    var eventIdKeyCol = new ForeignKeyColumn(eventIdKey, "event_id")
                    {
                        ReferencedColumn = "event_id"
                    };
                    eventIdKey.Columns.Add(eventIdKeyCol);
                    eventIdKey.ReferencedTable = "Event";

                    eventIdKey.Create();
                }
            }
        }

        // Practice Table
        {
            // Create new table
            var PracticeTable = new Table(temp, "Practice");

            // practice_id
            var practice_id = new Column(PracticeTable, "practice_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            PracticeTable.Columns.Add(practice_id);

            // Event Id
            var event_id = new Column(PracticeTable, "event_id", DataType.BigInt)
            {
                Nullable = false
            };
            PracticeTable.Columns.Add(event_id);

            if (!temp.Tables.Contains("Practice"))
            {
                PracticeTable.Create();

                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Practice] ADD CONSTRAINT PK_Practice PRIMARY KEY (practice_id);";
                temp.ExecuteNonQuery(sql);

                // Create the foreign key after the "PracticeTable" has been created
                {
                    PracticeTable = temp.Tables["Practice"]; // Retrieve the existing "PracticeTable"

                    // Event id                             Foriegn Key from Event
                    var eventIdKey = new ForeignKey(PracticeTable, "FK_Practice_Event");
                    var eventIdKeyCol = new ForeignKeyColumn(eventIdKey, "event_id")
                    {
                        ReferencedColumn = "event_id"
                    };
                    eventIdKey.Columns.Add(eventIdKeyCol);
                    eventIdKey.ReferencedTable = "Event";

                    eventIdKey.Create();
                }
            }
        }

        // Session Table
        {
            // Create new Table
            var SessionTable = new Table(temp, "Session");

            // Session ID
            var session_id = new Column(SessionTable, "session_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            SessionTable.Columns.Add(session_id);

            // league_id
            var league_id = new Column(SessionTable, "league_id", DataType.BigInt)
            {
                Nullable = false
            };
            SessionTable.Columns.Add(league_id);

            // tournament_id
            var tournament_id = new Column(SessionTable, "tournament_id", DataType.BigInt)
            {
                Nullable = false
            };
            SessionTable.Columns.Add(tournament_id);

            // practice_id
            var practice_id = new Column(SessionTable, "practice_id", DataType.BigInt)
            {
                Nullable = false
            };
            SessionTable.Columns.Add(practice_id);

            // Date
            var date = new Column(SessionTable, "date", DataType.DateTime)
            {
                Nullable = false
            };
            SessionTable.Columns.Add(date);

            // Location
            var location = new Column(SessionTable, "location", DataType.VarChar(255));
            SessionTable.Columns.Add(location);

            // Total Games
            var total_games = new Column(SessionTable, "total_games", DataType.BigInt);
            SessionTable.Columns.Add(total_games);

            // Total Frames
            var total_frames = new Column(SessionTable, "total_frames", DataType.BigInt);
            SessionTable.Columns.Add(total_frames);


            if (!temp.Tables.Contains("Session"))
            {
                SessionTable.Create();

                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Session] ADD CONSTRAINT PK_Session PRIMARY KEY (session_id);";
                temp.ExecuteNonQuery(sql);

                // Create the foreign keys after the "SessionTable" has been created
                {
                    SessionTable = temp.Tables["Session"]; // Retrieve the existing "SessionTable"

                    // league_id                            Foriegn Key from League
                    var leagueIdKey = new ForeignKey(SessionTable, "FK_Session_League");
                    var leagueIdKeyCol = new ForeignKeyColumn(leagueIdKey, "league_id")
                    {
                        ReferencedColumn = "league_id"
                    };
                    leagueIdKey.Columns.Add(leagueIdKeyCol);
                    leagueIdKey.ReferencedTable = "League";

                    leagueIdKey.Create();
                    //////////
                    ///
                    // tournament_id                            Foriegn Key from Tournament
                    var tournamentIdKey = new ForeignKey(SessionTable, "FK_Session_Tournament");
                    var tournamentIdKeyCol = new ForeignKeyColumn(tournamentIdKey, "tournament_id")
                    {
                        ReferencedColumn = "tournament_id"
                    };
                    tournamentIdKey.Columns.Add(tournamentIdKeyCol);
                    tournamentIdKey.ReferencedTable = "Tournament";

                    tournamentIdKey.Create();
                    //////////
                    ///
                    // Practice_id                            Foriegn Key from Practice
                    var practiceIdKey = new ForeignKey(SessionTable, "FK_Session_Practice");
                    var practiceIdKeyCol = new ForeignKeyColumn(practiceIdKey, "practice_id")
                    {
                        ReferencedColumn = "practice_id"
                    };
                    practiceIdKey.Columns.Add(practiceIdKeyCol);
                    practiceIdKey.ReferencedTable = "Practice";

                    practiceIdKey.Create();


                }

            }


        }

        // Game Table
        {
            // Create new Table
            var GameTable = new Table(temp, "Game");

            // Game id
            var game_id = new Column(GameTable, "game_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            GameTable.Columns.Add(game_id);

            // session_id
            var session_id = new Column(GameTable, "session_id", DataType.BigInt)
            {
                Nullable = false
            };
            GameTable.Columns.Add(session_id);

            var score = new Column(GameTable, "score", DataType.VarChar(255));
            GameTable.Columns.Add(score);


            if (!temp.Tables.Contains("Game"))
            {
                GameTable.Create();


                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Game] ADD CONSTRAINT PK_Game PRIMARY KEY (game_id);";
                temp.ExecuteNonQuery(sql);


                // Create the foreign key after the "GameTable" has been created
                {
                    GameTable = temp.Tables["Game"]; // Retrieve the existing "GameTable"

                    // Event id                             Foriegn Key from Session
                    var sessionIdKey = new ForeignKey(GameTable, "FK_Game_Session");
                    var sessionIdKeyCol = new ForeignKeyColumn(sessionIdKey, "session_id")
                    {
                        ReferencedColumn = "session_id"
                    };
                    sessionIdKey.Columns.Add(sessionIdKeyCol);
                    sessionIdKey.ReferencedTable = "Session";

                    sessionIdKey.Create();
                }

            }
        }

        // Frame Table
        {
            // Create new
            var FrameTable = new Table(temp, "Frame");

            // Frame_id
            var frame_id = new Column(FrameTable, "frame_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            FrameTable.Columns.Add(frame_id);

            // User ID
            var game_id = new Column(FrameTable, "game_id", DataType.BigInt)
            {
                Nullable = false
            };
            FrameTable.Columns.Add(game_id);

            var score = new Column(FrameTable, "score", DataType.VarChar(255));
            FrameTable.Columns.Add(score);


            if (!temp.Tables.Contains("Frame"))
            {
                FrameTable.Create();
            }

            // Create the primary key constraint using SQL
            string sql = "ALTER TABLE [Frame] ADD CONSTRAINT PK_Frame PRIMARY KEY (frame_id);";
            temp.ExecuteNonQuery(sql);

            // Create the foreign key after the "FrameTable" has been created
            {
                FrameTable = temp.Tables["Frame"]; // Retrieve the existing "FrameTable"

                // Game id                             Foriegn Key from Game
                var gameIdKey = new ForeignKey(FrameTable, "FK_Frame_Game");
                var gameIdKeyCol = new ForeignKeyColumn(gameIdKey, "game_id")
                {
                    ReferencedColumn = "game_id"
                };
                gameIdKey.Columns.Add(gameIdKeyCol);
                gameIdKey.ReferencedTable = "Game";

                gameIdKey.Create();
            }


        }

        // Video Table
        {
            // Create new table
            var VideoTable = new Table(temp, "Video");

            // practice_id
            var video_id = new Column(VideoTable, "video_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            VideoTable.Columns.Add(video_id);

            // Event Id
            var video = new Column(VideoTable, "video", DataType.VarBinaryMax)
            {
                Nullable = false
            };
            VideoTable.Columns.Add(video);

            if (!temp.Tables.Contains("Video"))
            {
                VideoTable.Create();


                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Video] ADD CONSTRAINT PK_Video PRIMARY KEY (video_id);";

                temp.ExecuteNonQuery(sql);
            }
        }

        // Ball Table
        {
            // Create new table
            var BallTable = new Table(temp, "Ball");

            // ball_id
            var ball_id = new Column(BallTable, "ball_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };
            BallTable.Columns.Add(ball_id);

            // weight
            var weight = new Column(BallTable, "weight", DataType.Float)
            {
                Nullable = false
            };
            BallTable.Columns.Add(weight);

            // color
            var color = new Column(BallTable, "color", DataType.VarChar(50));
            BallTable.Columns.Add(color);

            if (!temp.Tables.Contains("Ball"))
            {
                BallTable.Create();


                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Ball] ADD CONSTRAINT PK_Ball PRIMARY KEY (ball_id);";

                temp.ExecuteNonQuery(sql);
            }
        }

        // Shot Table
        {
            // Create new
            var ShotTable = new Table(temp, "Shot");

            // Shot_id Primary Key 
            // Primary Key is added after the table is created. 
            var shot_id = new Column(ShotTable, "shot_id", DataType.BigInt)
            {
                IdentityIncrement = 1,
                Nullable = false,
                IdentitySeed = 1,
                Identity = true
            };

            ShotTable.Columns.Add(shot_id);

            // Frame_id which is a Foreign Key
            // Foreign Key is added after the table is created.
            var frame_id = new Column(ShotTable, "frame_id", DataType.BigInt)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(frame_id);

            // Ball_id which is a Foreign Key
            // Foreign Key is added after the table is created.
            var ball_id = new Column(ShotTable, "ball_id", DataType.BigInt)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(ball_id);

            // Video_id which is a Foreign Key
            // Foreign Key is added after the table is created.
            var video_id = new Column(ShotTable, "video_id", DataType.BigInt)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(video_id);

            // Pins Remaining  
            var pins_remaining = new Column(ShotTable, "pins_remaining", DataType.Binary(2))
            {
                Nullable = false
            };
            ShotTable.Columns.Add(pins_remaining);

            // Time Remaining  
            var time = new Column(ShotTable, "time", DataType.DateTime)
            {
                Nullable = false
            };
            ShotTable.Columns.Add(time);

            // Lane Number  
            var lane_Number = new Column(ShotTable, "lane_Number", DataType.Binary(2))
            {
                Nullable = false
            };
            ShotTable.Columns.Add(lane_Number);

            // These will need to be updated somehow to be lists
            var ddx = new Column(ShotTable, "ddx", DataType.Float);
            ShotTable.Columns.Add(ddx);

            // These will need to be updated somehow to be lists
            var ddy = new Column(ShotTable, "ddy", DataType.Float);
            ShotTable.Columns.Add(ddy);

            // These will need to be updated somehow to be lists
            var ddz = new Column(ShotTable, "ddz", DataType.Float);
            ShotTable.Columns.Add(ddz);

            // These will need to be updated somehow to be lists
            var x_position = new Column(ShotTable, "x_position", DataType.Float);
            ShotTable.Columns.Add(x_position);

            // These will need to be updated somehow to be lists
            var y_position = new Column(ShotTable, "y_position", DataType.Float);
            ShotTable.Columns.Add(y_position);

            // These will need to be updated somehow to be lists
            var z_position = new Column(ShotTable, "z_position", DataType.Float);
            ShotTable.Columns.Add(z_position);

            // Create the entire table for shot
            if (!temp.Tables.Contains("Shot"))
            {
                ShotTable.Create();

                // Create the primary key constraint using SQL
                string sql = "ALTER TABLE [Shot] ADD CONSTRAINT PK_shot_id PRIMARY KEY (shot_id);";
                temp.ExecuteNonQuery(sql);
                // May need to add in the foreign keys?

                // Create the foreign key after the "ShotTable" has been created
                {
                    ShotTable = temp.Tables["Shot"]; // Retrieve the existing "ShotTable"

                    // Frame id                             Foriegn Key from Frame
                    var frameIdKey = new ForeignKey(ShotTable, "FK_Shot_Frame");
                    var frameIdKeyCol = new ForeignKeyColumn(frameIdKey, "frame_id")
                    {
                        ReferencedColumn = "frame_id"
                    };
                    frameIdKey.Columns.Add(frameIdKeyCol);
                    frameIdKey.ReferencedTable = "Frame";
                    frameIdKey.Create();

                    // Ball id                             Foriegn Key from Ball
                    var ballIdKey = new ForeignKey(ShotTable, "FK_Shot_Ball");
                    var ballIdKeyCol = new ForeignKeyColumn(ballIdKey, "ball_id")
                    {
                        ReferencedColumn = "ball_id"
                    };
                    ballIdKey.Columns.Add(ballIdKeyCol);
                    ballIdKey.ReferencedTable = "Ball";
                    ballIdKey.Create();

                    // Video id                             Foriegn Key from Video
                    var videoIdKey = new ForeignKey(ShotTable, "FK_Shot_Video");
                    var videoIdKeyCol = new ForeignKeyColumn(videoIdKey, "video_id")
                    {
                        ReferencedColumn = "video_id"
                    };
                    videoIdKey.Columns.Add(videoIdKeyCol);
                    videoIdKey.ReferencedTable = "Video";
                    videoIdKey.Create();

                }

            }

        }


    }

    public async Task Kill()
    {

            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            // This will need to be adjusted to see if constraints are being reset in order.
            // As well as the order of Dropping the table 

            // Get Rid of The Key Constraints for Shot
            string noConstraint = "Use [revmetrix-u] ALTER TABLE [Shot] NOCHECK CONSTRAINT all";
            using var command1 = new SqlCommand(noConstraint, connection);
            command1.ExecuteNonQuery();

            // Dropping the Shot Table
            string dropShot = "DROP TABLE [Shot]";
            using var command2 = new SqlCommand(dropShot, connection);
            command2.ExecuteNonQuery();

            // Dropping the Video Table
            string dropVideo = "DROP TABLE [Video]";
            using var command3 = new SqlCommand(dropVideo, connection);
            command3.ExecuteNonQuery();

            // Dropping the Ball Table
            string dropBall = "DROP TABLE [Ball]";
            using var command4 = new SqlCommand(dropBall, connection);
            command4.ExecuteNonQuery();

            // Dropping the Frame Table
            string dropFrame = "DROP TABLE [Frame]";
            using var command5 = new SqlCommand(dropFrame, connection);
            command5.ExecuteNonQuery();

            // Dropping the Game Table
            string dropGame = "DROP TABLE [Game]";
            using var command6 = new SqlCommand(dropGame, connection);
            command6.ExecuteNonQuery();

            // Dropping the Session Table
            string dropSession = "DROP TABLE [Session]";
            using var command7 = new SqlCommand(dropSession, connection);
            command7.ExecuteNonQuery();

            // Dropping the League Table
            string dropLeague = "DROP TABLE [League]";
            using var command8 = new SqlCommand(dropLeague, connection);
            command8.ExecuteNonQuery();

            // Dropping the Tournament Table
            string dropTournament = "DROP TABLE [Tournament]";
            using var command9 = new SqlCommand(dropTournament, connection);
            command9.ExecuteNonQuery();

            // Dropping the Practice Table
            string dropPractice = "DROP TABLE [Practice]";
            using var command10 = new SqlCommand(dropPractice, connection);
            command10.ExecuteNonQuery();

            // Dropping the Event Table 
            string dropEvent = "DROP TABLE [Event]";
            using var command11 = new SqlCommand(dropEvent, connection);
            command11.ExecuteNonQuery();

            // Dropping the RefreshToken Table
            string dropRefreshToken = "DROP TABLE [RefreshToken]";
            using var command12 = new SqlCommand(dropRefreshToken, connection);
            command12.ExecuteNonQuery();

            // Droping the User Table
            string dropUser = "DROP TABLE [User]";
            using var command13 = new SqlCommand(dropUser, connection);
            command13.ExecuteNonQuery();



















    }

    public async Task<bool> AddUser(string username, byte[] hashedPassword, byte[] salt, string roles, string phone, string email)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [User] (username, salt, roles, password, email, phone) " +
                             "VALUES (@Username, @Salt, @Roles, @Password, @Email, @Phone)";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
        command.Parameters.Add("@Salt", SqlDbType.VarBinary, 16).Value = salt;
        command.Parameters.Add("@Roles", SqlDbType.VarChar).Value = roles;
        command.Parameters.Add("@Password", SqlDbType.VarBinary, -1).Value = hashedPassword;
        command.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;
        command.Parameters.Add("@Phone", SqlDbType.VarChar).Value = phone;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public async Task<bool> RemoveUser(string username)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string deleteQuery = "DELETE FROM [User] WHERE username = @Username";

        using var command = new SqlCommand(deleteQuery, connection);
        command.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<(bool success, string roles)> GetRoles(string username)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string selectQuery = "SELECT roles FROM [User] WHERE username = @Username";

        using var command = new SqlCommand(selectQuery, connection);
        command.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;

        using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            // Retrieve the "roles" value
            string roles = reader["roles"].ToString();

            return (true, roles);
        }
        else
        {
            return (false, null);
        }
    }

    public async Task<(bool success, byte[] salt, string roles, byte[] hashedPassword)> GetUserValidData(string username)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string selectQuery = "SELECT roles, password, salt FROM [User] WHERE username = @Username";

        using var command = new SqlCommand(selectQuery, connection);
        command.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;

        using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            // Retrieve the columns
            string db_roles = reader["roles"].ToString();
            byte[] db_hashedPassword = (byte[])reader["password"]; ;
            byte[] db_salt = (byte[])reader["salt"];

            return (true, db_salt, db_roles, db_hashedPassword);
        }
        else
        {
            return (false, null, null, null);
        }
    }

    public async Task<bool> AddRefreshToken(byte[] token, string username, DateTime expiration)
    {
        // Look into user table, get id that matches with username
        // add row to token table (token, id, expiration)
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [Token] (token, userid, expiration) " +
            "VALUES (@Token, (SELECT id FROM [User] WHERE username = @Username), @Expiration);";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
        command.Parameters.Add("@Token", SqlDbType.VarBinary, 32).Value = token;
        command.Parameters.Add("@Expiration", SqlDbType.DateTime).Value = expiration;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public async Task<(bool success, string username, DateTime? expiration)> RemoveRefreshToken(byte[] token)
    {
        // Remove row where token matches token param
        // Retrieve username and expiration at same time

        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "DELETE FROM [Token] " +
            "OUTPUT DELETED.expiration, [User].username " +
            "FROM [Token] " +
            "INNER JOIN [User] ON [Token].userid = [User].id " +
            "WHERE [Token].token = @Token;";

        using var command = new SqlCommand(insertQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@Token", SqlDbType.VarBinary, 32).Value = token;

        // Execute the query
        using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            // Retrieve the columns
            string username = reader["username"].ToString();
            var expiration = (DateTime)reader["expiration"];

            return (true, username, expiration);
        }
        else
        {
            return (false, null, null);
        }
    }

    public async Task<bool> RemoveRelatedRefreshTokens(string username)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string deleteQuery = """
                    DELETE FROM [Token]
                    WHERE userid = (SELECT id FROM [User] WHERE username = @Username);
                    """;

        using var command = new SqlCommand(deleteQuery, connection);
        // Set the parameter values
        command.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }

    public bool DoesExist()
    {
        database = new Microsoft.SqlServer.Management.Smo.Database(Server, DatabaseName);
        if (!Server.Databases.Contains(DatabaseName))
        {
            return true;
        }
        // Otherwise breakout of createTables
        else
        {
            return false;
        }
    }

}
