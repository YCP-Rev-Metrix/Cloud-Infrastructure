using Common.Logging;
using Common.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;
// Authorize Refresh Register
// Logout Unregister ChangePassword

[ApiController]
[Route("api/[controller]")]
public class UserController : AbstractFeaturedController
{
    [HttpPost("Authorize", Name = "Authorize")]
    public async Task<IActionResult> Authorize([FromBody] UserIdentification userIdentification)
    {
        LogWriter.LogInfo("Authorize called");
        // Validate user credentials (e.g., check against a database)
        (bool success, string[]? roles) = await ServerState.UserStore.VerifyUser(userIdentification.Username, userIdentification.Password);
        if (success)
        {
            (string authorizationToken, byte[] refreshToken) = await ServerState.TokenStore.GenerateTokenSet(userIdentification.Username, roles);

            // Return the token as a response
            return Ok(new DualToken(authorizationToken, refreshToken));
        }

        // If credentials are invalid, return a 403 Forbid response
        return Forbid();
    }

    [HttpPost("Refresh", Name = "Refresh")]
    public async Task<IActionResult> Refresh([FromBody] ByteArrayToken refreshToken)
    {
        LogWriter.LogInfo("Refresh called");
        (bool valid, string? username) = await ServerState.TokenStore.RemoveAndVerifyRefreshToken(refreshToken.Token);

        if (valid && username != null)
        {
            (bool success, string[]? roles) = await ServerState.UserStore.GetRoles(username);
            if (success)
            {
                (string authorization, byte[] refresh) = await ServerState.TokenStore.GenerateTokenSet(username, roles ?? Array.Empty<string>());
                return Ok(new DualToken(authorization, refresh));
            }
            else
            {
                // If credentials are invalid, return a 403 Forbid response
                return Forbid();
            }
        }

        return Unauthorized();
    }

    [HttpPost("Register", Name = "Register")]
    public async Task<IActionResult> Register([FromBody] UserIdentification userIdentification)
    {
        LogWriter.LogInfo("Register called");
        bool result = await ServerState.UserStore.CreateUser(userIdentification.Username, userIdentification.Password, new string[] { "user" });

        return result ? await Authorize(userIdentification) : Conflict();
    }

    [Authorize]
    [HttpDelete("Logout", Name = "Logout")]
    public async Task<IActionResult> Logout()
    {
        LogWriter.LogInfo("Logout called");
        string? username = GetUsername();
        if (username != null)
        {
            await ServerState.TokenStore.RemoveRelatedRefreshTokens(username);
        }

        string? jwt = GetJWT();
        if (jwt != null)
        {
            ServerState.TokenStore.BlacklistAuthorizationToken(jwt);
        }

        return Ok();
    }

    [Authorize]
    [HttpPost("Unregister", Name = "Unregister")]
    public IActionResult Unregister([FromBody] UserIdentification userIdentification) => throw new NotImplementedException();
}
