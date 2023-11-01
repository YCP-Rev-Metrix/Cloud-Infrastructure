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
    public IActionResult Authorize([FromBody] UserIdentification userIdentification)
    {
        // Validate user credentials (e.g., check against a database)
        if (ServerState.UserStore.VerifyUser(userIdentification.Username, userIdentification.Password, out string[]? roles))
        {
            var (authorizationToken, refreshToken) = ServerState.TokenStore.GenerateTokenSet(userIdentification.Username, roles);

            // Return the token as a response
            return Ok(new DualToken(authorizationToken, refreshToken));
        }

        // If credentials are invalid, return a 403 Forbid response
        return Forbid();
    }

    [HttpPost("Refresh", Name = "Refresh")]
    public IActionResult Refresh([FromBody] SingleToken refreshToken)
    {
        if (ServerState.TokenStore.RemoveAndVerifyRefreshToken(refreshToken.Token, out string? username, out string? newRefreshToken))
        {
            if (ServerState.UserStore.GetRoles(username, out string[]? roles))
            {
                string newAuthorizationToken = ServerState.TokenStore.GenerateAuthorizationToken(username, roles);
                return Ok(new DualToken(newAuthorizationToken, newRefreshToken));
            } else
            {
                // If credentials are invalid, return a 403 Forbid response
                return Forbid();
            }
        }

        return Unauthorized();
    }

    [HttpPost("Register", Name = "Register")]
    public IActionResult Register([FromBody] UserIdentification userIdentification)
    {
        if (ServerState.UserStore.CreateUser(userIdentification.Username, userIdentification.Password, new string[] { "user" })) {
            return Authorize(userIdentification);
        }

        return Conflict();
    }

    [Authorize]
    [HttpDelete("Logout", Name = "Logout")]
    public IActionResult Logout()
    {
        string? username = GetUsername();
        if (username != null)
        {
            ServerState.TokenStore.RemoveRelatedRefreshTokens(username);
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
    public IActionResult Unregister([FromBody] UserIdentification userIdentification)
    {
        throw new NotImplementedException();
    }
}
