using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace DatabaseCore;

public class UserDB : AbstractDatabase
{
    public UserDB() : base("revmetrix-u")
    {

    }

    public void CreateTables()
    {
        Database = new Microsoft.SqlServer.Management.Smo.Database(Server, DatabaseName);
        Database.Create();

        // User Table
        {
            // Create new
            var UserTable = new Table(Database, "User");

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

            UserTable.Create();

            // Create the primary key constraint using SQL
            string sql = "ALTER TABLE [User] ADD CONSTRAINT PK_User PRIMARY KEY (id);";
            Database.ExecuteNonQuery(sql);

            sql = "ALTER TABLE [User] ADD CONSTRAINT UNQ__User__username UNIQUE ([username])";
            Database.ExecuteNonQuery(sql);
        }

        // Token Table
        {
            // Create new
            var TokenTable = new Table(Database, "Token");

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

            TokenTable.Create();

            // Create the foreign key after the "TokenTable" has been created
            {
                TokenTable = Database.Tables["Token"]; // Retrieve the existing "TokenTable"

                // User ID
                var userIdKey = new ForeignKey(TokenTable, "FK_Token_User");
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

    public void Kill() => Database?.DropIfExists();

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
}
