using Common.Logging;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatabaseCore.DatabaseComponents;

public partial class RevMetrixDB
{
    public async Task<bool> Startframe(int game_id, int score)
    {
        ConnectionString = Environment.GetEnvironmentVariable("SERVERDB_CONNECTION_STRING");

        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        string InsertQuery = "INSERT INTO [Frame] (game_id, score)" + "VALUES (@Game_id, @Score)";

        using var command = new SqlCommand(InsertQuery, connection);
        command.Parameters.Add("@Game_id", SqlDbType.BigInt).Value = game_id;
        command.Parameters.Add("@Score", SqlDbType.BigInt).Value = score;

        int i = await command.ExecuteNonQueryAsync();
        return i != -1;

    }
}