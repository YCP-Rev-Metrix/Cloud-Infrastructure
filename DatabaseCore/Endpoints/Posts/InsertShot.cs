using Common.Logging;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatabaseCore.DatabaseComponents;

public partial class RevMetrixDB
{
    public async Task<bool> InsertShot(long user_id,
                                   long? frame_id,
                                   long? ball_id,
                                   long? video_id,
                                   long? pins_remaining, 
                                   DateTime time,
                                   long? lane_number,
                                   double ddx,
                                   double ddy,
                                   double ddz,
                                   double x_position,
                                   double y_position,
                                   double z_position,
                                   long? pocket_hit
        )
    {
        ConnectionString = Environment.GetEnvironmentVariable("SERVERDB_CONNECTION_STRING");

        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO [Shot] (user_id, frame_id, ball_id, video_id, pins_remaining, time, lane_number, ddx, ddy, ddz, x_position, y_position, z_position, pocket_hit) " +
                     "VALUES (@User_id, @Frame_id, @Ball_id, @Video_id, @Pins_remaining, @Time, @Lane_number, @Ddx , @Ddy, @Ddz, @X_position, @Y_position, @Z_position, @pocket_hit)";

        using var command = new SqlCommand(insertQuery, connection);

        // Set the parameter values
        command.Parameters.Add("@User_id", SqlDbType.BigInt).Value = user_id;

        // If frame_id is null
        if (frame_id > -1)
        {
            command.Parameters.Add("@Frame_id", SqlDbType.BigInt).Value = frame_id;
        }
        else
        {
            command.Parameters.Add("@Frame_id", SqlDbType.BigInt).Value = DBNull.Value;
        }
        // If ball_id is null
        if (ball_id > -1)
        {
            command.Parameters.Add("@Ball_id", SqlDbType.BigInt).Value = ball_id;
        }
        else
        {
            command.Parameters.Add("@Ball_id", SqlDbType.BigInt).Value = DBNull.Value;
        }
        // If video_id is null
        if (video_id > -1)
        {
            command.Parameters.Add("@Video_id", SqlDbType.BigInt).Value = video_id;
        }
        else
        {
            command.Parameters.Add("@Video_id", SqlDbType.BigInt).Value = DBNull.Value;
        }
        command.Parameters.Add("@Pins_remaining", SqlDbType.BigInt, 8).Value = pins_remaining;
        command.Parameters.Add("@Time", SqlDbType.DateTime, 2).Value = time;
        command.Parameters.Add("@Lane_number", SqlDbType.BigInt, 8).Value = lane_number;
        command.Parameters.Add("@Ddx", SqlDbType.Float).Value = ddx;
        command.Parameters.Add("@Ddy", SqlDbType.Float).Value = ddy;
        command.Parameters.Add("@Ddz", SqlDbType.Float).Value = ddz;
        command.Parameters.Add("@X_position", SqlDbType.Float).Value = x_position;
        command.Parameters.Add("@Y_position", SqlDbType.Float).Value = y_position;
        command.Parameters.Add("@Z_position", SqlDbType.Float).Value = z_position;
        command.Parameters.Add("@Pocket_hit", SqlDbType.BigInt).Value = pocket_hit;

        // Execute the query
        int i = await command.ExecuteNonQueryAsync();
        return i != -1;
    }
}