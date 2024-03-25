using Common.Logging;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatabaseCore.DatabaseComponents;

public partial class RevMetrixDB
{
    public async Task<bool> AddUser(string firstname, string lastname, string username, byte[] hashedPassword, byte[] salt, string roles, string phone, string email)
    {
        // If not local use this connection
        ConnectionString = "Server=143.110.146.58,1433;Database=revmetrix-db;User Id=SA;Password=BigPass@Word!;TrustServerCertificate=True;";
        // If local using this connection string
        //ConnectionString = "Data Source=localhost;Database=revmetrix-db;Integrated Security=True;TrustServerCertificate=True;";
        using var connection1 = new SqlConnection(ConnectionString);
        await connection1.OpenAsync();
        LogWriter.LogInfo(connection1);
        // ConnectionString = "Server = 143.110.146.58,1433; User Id = SA; Password = BigPass@Word!; TrustServerCertificate = True;";

        string insertQuery = "INSERT INTO [User] (firstname, lastname, username, salt, roles, password, email, phone) " +
                             "VALUES (@Firstname, @Lastname, @Username, @Salt, @Roles, @Password, @Email, @Phone)";

        using var command = new SqlCommand(insertQuery, connection1);
        // Set the parameter values
        command.Parameters.Add("@Firstname", SqlDbType.VarChar).Value = firstname;
        command.Parameters.Add("@Lastname", SqlDbType.VarChar).Value = lastname;
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
}