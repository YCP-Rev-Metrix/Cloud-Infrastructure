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
        bool result = await ServerState.UserStore.CreateUser(userIdentification.Username, userIdentification.Password, new string[] { "user" });

        return result ? await Authorize(userIdentification) : Conflict();
    }

    [Authorize]
    [HttpDelete("Logout", Name = "Logout")]
    public async Task<IActionResult> Logout()
    {
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


    [HttpPost("GetData", Name = "GetData")]
    public async Task<IActionResult> GetData([FromBody] InsertUserData userIdentification)
    {
        // return the username from the db where you give the user id
        (bool success, string? result) = await ServerState.UserStore.GetUsername(userIdentification.Userid);
        // return as a jsonresult
        return new JsonResult(result);
    }

    [HttpPost("Insert", Name = "Insert")]
    public async Task<IActionResult> Insert([FromBody] InsertUserData userIdentification)
    {
        // Validate user credentials (e.g., check against a database)

        bool result = await ServerState.UserStore.InsertUser(userIdentification.Userid,
                                                            userIdentification.Firstname,
                                                            userIdentification.Lastname,
                                                            userIdentification.Username,
                                                            userIdentification.Password,
                                                            userIdentification.Email,
                                                            userIdentification.Phone,
                                                            userIdentification.Role);

        return new JsonResult(result);
    }
}
