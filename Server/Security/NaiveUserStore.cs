using System.Diagnostics.CodeAnalysis;

namespace Server.Security;

public class NaiveUserStore : AbstractUserStore
{
    public readonly Dictionary<string, (string password, string[]? roles)> users;

    public NaiveUserStore(Dictionary<string, (string password, string[]? roles)> initialUsers) => users = initialUsers;

    public NaiveUserStore() => users = new();

    public override bool CreateUser(string username, string password, string[]? roles = null) => !users.ContainsKey(username) && users.TryAdd(username, (password, roles));

    public override bool DeleteUser(string username) => users.Remove(username);

    public override bool GetRoles(string username, [MaybeNullWhen(true)] out string[]? roles)
    {
        roles = null;

        if (users.TryGetValue(username, out (string password, string[]? roles) output))
        {
            roles = output.roles;
            return true;
        }

        return false;
    }

    public override bool VerifyUser(string username, string password, [MaybeNullWhen(true)] out string[]? roles)
    {
        roles = null;

        if (users.TryGetValue(username, out (string password, string[]? roles) output))
        {
            if (password == output.password)
            {
                roles = output.roles;
                return true;
            }
        }

        return false;
    }
}
