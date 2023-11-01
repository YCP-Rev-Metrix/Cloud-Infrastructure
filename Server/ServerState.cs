using Server.Security;

namespace Server;

/// <summary>
/// Provides a static server state to reference against. Items such as the following:
/// <list type="bullet">
/// <item><see cref="TokenStore"/></item>
/// </list>
/// </summary>
public static class ServerState
{
    public static readonly AbstractTokenStore TokenStore = new NaiveTokenStore(TimeSpan.FromMinutes(60), TimeSpan.FromHours(24), TimeSpan.FromMinutes(5));

    public static readonly AbstractUserStore UserStore = new NaiveUserStore(new()
    {
        {"user", ("pass", new string[] {"user" })},
        {"admin", ("root", new string[] {"admin" }) }
    });
}
