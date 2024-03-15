using Common.Logging;
using Common.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

/// <summary>
/// Provides much of the functionality surrounding users, authentication, account creation, and more
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : AbstractFeaturedController
{
    /// <summary>
    /// Authorizes a requests provided credentials agains the user database
    /// </summary>
    /// <param name="userIdentification">The user's information, only username and password must be set for authorization</param>
    /// <returns><see cref="StatusCodes.Status200OK"/>(<see cref="DualToken"/>) | <see cref="StatusCodes.Status403Forbidden"/></returns>
    [HttpPost("Authorize", Name = "Authorize")]
    [ProducesResponseType(typeof(DualToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Authorize([FromBody] UserIdentification userIdentification)
    {
        LogWriter.LogInfo("Authorize called");

        // Validate user credentials (e.g., check against a database)
        (bool success, string[]? roles) = await ServerState.UserStore.VerifyUser(userIdentification.Username, userIdentification.Password);
        if (success)
        {
            // Generate a token set (auth & refresh) from the user's information
            (string authorizationToken, byte[] refreshToken) = await ServerState.TokenStore.GenerateTokenSet(userIdentification.Username, roles);

            // Return the tokens as a response
            return Ok(new DualToken(authorizationToken, refreshToken));
        }

        // If credentials are invalid, return a 403 Forbid response
        return Forbid();
    }

    /// <summary>
    /// Allows a user to exchange their refresh token for a new JWT and refresh token
    /// </summary>
    /// <param name="refreshToken">User's old refresh token</param>
    /// <returns><see cref="StatusCodes.Status200OK"/>(<see cref="DualToken"/>) | <see cref="StatusCodes.Status403Forbidden"/> | <see cref="StatusCodes.Status401Unauthorized"/></returns>
    [HttpPost("Refresh", Name = "Refresh")]
    [ProducesResponseType(typeof(DualToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] ByteArrayToken refreshToken)
    {
        LogWriter.LogInfo("Refresh called");

        // Grab username and verify refresh token
        (bool valid, string? username) = await ServerState.TokenStore.RemoveAndVerifyRefreshToken(refreshToken.Token);

        // Ensure validity
        if (valid && username != null)
        {
            // Retrieve roles for this user
            (bool success, string[]? roles) = await ServerState.UserStore.GetRoles(username);
            if (success)
            {
                // Generate a new auth and refresh token
                (string authorization, byte[] refresh) = await ServerState.TokenStore.GenerateTokenSet(username, roles ?? Array.Empty<string>());

                // Return the tokens as a response
                return Ok(new DualToken(authorization, refresh));
            }
            else
            {
                // If credentials are invalid, return a 403 Forbid response
                return Forbid();
            }
        }

        // Return unauthorized in the event that the refresh token was invalid
        return Unauthorized();
    }

    /// <summary>
    /// Allows someone to register for a new account in the database. Will then (if account created) authorize the user and provide their refresh token and JWT
    /// </summary>
    /// <param name="userIdentification">All user identification provided by the user</param>
    /// <returns><see cref="StatusCodes.Status200OK"/>(<see cref="DualToken"/>) | <see cref="StatusCodes.Status403Forbidden"/> | <see cref="StatusCodes.Status409Conflict"/></returns>
    [HttpPost("Register", Name = "Register")]
    [ProducesResponseType(typeof(DualToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] UserIdentification userIdentification)
    {
        LogWriter.LogInfo("Register called");
        bool result = await ServerState.UserStore.CreateUser(userIdentification.Firstname,
                                                              userIdentification.Lastname,
                                                              userIdentification.Username,
                                                              userIdentification.Password,
                                                              userIdentification.Email,
                                                              userIdentification.Phone_number,
                                                              new string[] { "user" });

        return result ? await Authorize(userIdentification) : Conflict();
    }

    /// <summary>
    /// Logs a user out of their account by removing their refresh tokens and blacklisting their JWT
    /// </summary>
    /// <returns><see cref="StatusCodes.Status200OK"/></returns>
    [Authorize]
    [HttpDelete("Logout", Name = "Logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    /// <summary>
    /// Unregister's a user's account and removes their access permentantly
    /// </summary>
    /// <param name="userIdentification">The user's identification (username and password)</param>
    /// <returns><see cref="StatusCodes.Status500InternalServerError"/></returns>
    /// <exception cref="NotImplementedException"></exception>
    [Authorize]
    [HttpPost("Unregister", Name = "Unregister")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Unregister([FromBody] UserIdentification userIdentification) => throw new NotImplementedException();

    [HttpPost("InsertShot", Name = "InsertShot")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]

    public async Task<IActionResult> InsertShot([FromBody] Shot shot)
    {
        
        return Ok(await ServerState.UserStore.InsertShot(shot.User_id,
                                                      shot.Frame_id,
                                                      shot.Ball_id,
                                                      shot.Video_id,
                                                      //shot.Pins_remaining,
                                                      shot.Time,
                                                      //shot.Lane_Number,
                                                      shot.Ddx,
                                                      shot.Ddy,
                                                      shot.Ddz,
                                                      shot.X_position,
                                                      shot.Y_position,
                                                      shot.Z_position
                                                      ));
    }


    [HttpPost("InsertBall", Name = "InsertBall")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertBall([FromBody] Ball ball)
    {
        return Ok( await ServerState.UserStore.InsertBall(ball.Weight, ball.Color));
    }

    [HttpPost("StartSession", Name = "StartSession")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartSession([FromBody] Session session)
    {
        return Ok(await ServerState.UserStore.StartSession(session.Leauge_id,
                                                           session.Tournament_id,
                                                           session.Practice_id,
                                                           session.Time,
                                                           session.Location,
                                                           session.Total_Games,
                                                           session.Total_Frames));
    }

    [HttpPost("StartPractice", Name = "StartPractice")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartPractice([FromBody] Practice practice)
    {
        return Ok(await ServerState.UserStore.StartPractice(practice.Event_id));
    }

    [HttpPost("StartEvent", Name = "StartEvent")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartEvent([FromBody] Event events )
    {
        return Ok(await ServerState.UserStore.StartEvent(events.User_id, events.Event_type));
    }

    [HttpPost("StartGame", Name = "StartGame")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartGame([FromBody] Game game)
    {
        return Ok(await ServerState.UserStore.StartGame(game.Session_id, game.Score));
    }

    [HttpPost("StartFrame", Name = "StartFrame")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartFrame([FromBody] Frame frame)
    {
        return Ok(await ServerState.UserStore.StartFrame(frame.Game_id, frame.Score));
    }

}
