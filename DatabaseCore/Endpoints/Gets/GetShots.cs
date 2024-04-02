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

        string selectQuery = "SELECT user_id, frame_id, ball_id, video_id, pins_remaining, time, lane_number, ddx, ddy, ddz, x_position, y_position, z_position FROM [Shot]"; // select all shots

        using var command = new SqlCommand(selectQuery, connection);

        var shots = new List<Shot>();
        using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync()) // Use while instead of if to handle multiple rows
        {

            var pinsRemaining = new byte[2];
            reader.GetBytes(reader.GetOrdinal("pins_remaining"), 0, pinsRemaining, 0, 2);

            var laneNumber = new byte[2];
            reader.GetBytes(reader.GetOrdinal("lane_number"), 0, laneNumber, 0, 2);

            // construct a new Shot object for each entry
            var shot = new Shot(
                user_id: reader.GetInt32(reader.GetOrdinal("user_id")),
                frame_id: reader.IsDBNull(reader.GetOrdinal("frame_id")) ? null : reader.GetInt32(reader.GetOrdinal("frame_id")),
                ball_id: reader.IsDBNull(reader.GetOrdinal("ball_id")) ? null : reader.GetInt32(reader.GetOrdinal("ball_id")),
                video_id: reader.IsDBNull(reader.GetOrdinal("video_id")) ? null : reader.GetInt32(reader.GetOrdinal("video_id")),
                pins_remaining: pinsRemaining,
                time: reader.GetDateTime(reader.GetOrdinal("time")),
                lane_number: laneNumber,
                ddx: reader.GetFloat(reader.GetOrdinal("ddx")),
                ddy: reader.GetFloat(reader.GetOrdinal("ddy")),
                ddz: reader.GetFloat(reader.GetOrdinal("ddz")),
                x_position: reader.GetFloat(reader.GetOrdinal("x_position")),
                y_position: reader.GetFloat(reader.GetOrdinal("y_position")),
                z_position: reader.GetFloat(reader.GetOrdinal("z_position"))
            );

            shots.Add(shot);
        }

        return (shots.Any(), shots);
    }
}