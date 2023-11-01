using System.Diagnostics.CodeAnalysis;

namespace Server.Security;

public abstract class AbstractUserStore
{
    public abstract bool CreateUser(string username, string password, string[]? roles = null);
    public abstract bool DeleteUser(string username);

    public abstract bool GetRoles(string username, [MaybeNullWhen(true)] out string[]? roles);
    public abstract bool VerifyUser(string username, string password, [MaybeNullWhen(true)] out string[]? roles);
}
