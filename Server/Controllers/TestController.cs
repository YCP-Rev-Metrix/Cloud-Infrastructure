using Common.Logging;
using Common.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

/// <summary>
/// Provides functionality for testing API connections
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : AbstractFeaturedController
{
    /// <summary>
    /// Basic test - should recieve a 200 code
    /// </summary>
    /// <returns><see cref="StatusCodes.Status200OK"/></returns>
    [HttpGet("Test", Name = "Test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Test()
    {
        LogWriter.LogInfo("Test called");
        return Ok();
    }

    /// <summary>
    /// Tests to ensure that the accessing user is authenticated with a JWT
    /// </summary>
    /// <returns><see cref="StatusCodes.Status200OK"/> | <see cref="StatusCodes.Status403Forbidden"/></returns>
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet("TestAuthorize", Name = "TestAuthorize")]
    public IActionResult TestAuthorize()
    {
        LogWriter.LogInfo("TestAuthorize called");
        return Ok();
    }

    /// <summary>
    /// Tests to ensure that the accessing user is authenticated with a JWT with the admin role
    /// </summary>
    /// <returns><see cref="StatusCodes.Status200OK"/> | <see cref="StatusCodes.Status403Forbidden"/> | <see cref="StatusCodes.Status401Unauthorized"/></returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("TestAuthenticateAdmin", Name = "TestAuthenticateAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult TestAuthenticateAdmin()
    {
        LogWriter.LogInfo("TestAuthenticateAdmin called");
        return Ok();
    }

    /// <summary>
    /// Grabs the current server time in UTC and send it back - good for testing clock sync and connection
    /// </summary>
    /// <returns><see cref="StatusCodes.Status200OK"/>(<see cref="DateTimePoco"/>)</returns>
    [HttpGet("TestTime", Name = "TestTime")]
    [ProducesResponseType(typeof(DateTimePoco), StatusCodes.Status200OK)]
    public IActionResult TestTime()
    {
        LogWriter.LogInfo("TestTime called");
        return Ok(DateTimePoco.UTCNow);
    }
}
