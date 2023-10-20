﻿using Common.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        (bool valid, string? username) removed = await ServerState.TokenStore.RemoveAndVerifyRefreshToken(refreshToken.Token);

        if (removed.valid && removed.username != null)
        {
            (bool success, string[]? roles) = await ServerState.UserStore.GetRoles(removed.username);
            if (success)
            {
                (string authorization, byte[] refresh) = await ServerState.TokenStore.GenerateTokenSet(removed.username, roles ?? Array.Empty<string>());
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
    public async Task<IActionResult> Unregister([FromBody] UserIdentification userIdentification) => throw new NotImplementedException();
}
