namespace Server.Security;

public abstract class AbstractUserStore
{
    public abstract Task<bool> CreateUser(string firstname, string lastname, string username, string password, string email, string phone_number, string[]? roles = null));
    public abstract Task<bool> DeleteUser(string username);

    public abstract Task<(bool success, string[]? roles)> GetRoles(string username);
    public abstract Task<(bool success, string[]? roles)> VerifyUser(string username, string password);
}
