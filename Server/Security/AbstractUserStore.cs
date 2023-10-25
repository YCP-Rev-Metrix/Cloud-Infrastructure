namespace Server.Security;

public abstract class AbstractUserStore
{
    public abstract Task<bool> CreateUser(string username, string password, string[]? roles = null);
    public abstract Task<bool> DeleteUser(string username);

    // Jordan adding a insert 
    public abstract Task<bool> InsertUser(int userid, string firstname, string lastname, string username, string password, string email, string phone  , string[]? roles = null);
    public abstract Task<(bool success, string[]? roles)> GetRoles(string username);
    public abstract Task<(bool success, string[]? roles)> VerifyUser(string username, string password);
}
