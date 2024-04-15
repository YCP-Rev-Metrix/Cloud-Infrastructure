using Common.Logging;
using Common.POCOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatabaseCore.DatabaseComponents;

public partial class RevMetrixDB
{
    public async Task<(bool success, List<Shot> shots)> GetShots()
    {
        ConnectionString = Environment.GetEnvironmentVariable("SERVERDB_CONNECTION_STRING");
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string selectQuery = "SELECT user_id, frame_id, ball_id, video_id, pins_remaining, time, lane_number, ddx, ddy, ddz, x_position, y_position, z_position, pocket_hit FROM [Shot]"; // select all shots

        using var command = new SqlCommand(selectQuery, connection);

        var shots = new List<Shot>();
        using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync()) // Use while instead of if to handle multiple rows
        {
            // construct a new Shot object for each entry
            var shot = new Shot(
                user_id: (int)reader.GetInt64("user_id"),
                frame_id: (int)reader.GetInt64("frame_id"),
                ball_id: (int)reader.GetInt64("ball_id"),
                video_id: (int)reader.GetInt64("video_id"),
                pins_remaining: (int)reader.GetInt64("pins_remaining"),
                time: reader.GetDateTime("time"),
                lane_number: (int)reader.GetInt64("lane_number"),
                ddx: reader.GetFloat("ddx"),
                ddy: reader.GetFloat("ddy"),
                ddz: reader.GetFloat("ddz"),
                x_position: reader.GetFloat("x_position"),
                y_position: reader.GetFloat("y_position"),
                z_position: reader.GetFloat("z_position"),
                pocket_hit: (int)reader.GetInt64("pocket_hit")
            );

            shots.Add(shot);
        }

        return (shots.Any(), shots);
    }
}