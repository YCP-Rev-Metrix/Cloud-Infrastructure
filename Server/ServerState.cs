using DatabaseCore;
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
    public static readonly SecurityHandler SecurityHandler = new();

    public static readonly UserDB UserDatabase = new();

    public static readonly ResearchDB ResearchDatabase = new();

    public static readonly AbstractTokenStore TokenStore = new DatabaseTokenStore(TimeSpan.FromMinutes(60), TimeSpan.FromHours(24), TimeSpan.FromMinutes(5));

    public static readonly AbstractUserStore UserStore = new DatabaseUserStore();
}
