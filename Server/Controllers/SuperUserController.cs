using Common.POCOs;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

/// <summary>
/// Provides elevated admin functionality such as database rebuilding
/// </summary>
public class SuperUserController : AbstractFeaturedController
{
    private readonly byte[] Salt = Convert.FromBase64String("tu99J/MoR/0fPqiANiUSsQ==");
    private readonly byte[] HashedPassword = Convert.FromBase64String("f389lt8C+LGKL8x02bqt3QKP+FUFMdPchLesmSeHgMY=");

    /// <summary>
    /// Destroys and re-builds both the User and Research databases with appropriate tables (removes all data)
    /// </summary>
    /// <param name="password">Password to access this endpoint</param>
    /// <returns><see cref="StatusCodes.Status200OK"/> | <see cref="StatusCodes.Status403Forbidden"/></returns>
    [HttpPost("ByeBye", Name = "ByeBye")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult ByeBye([FromBody] Password password)
    {
        // Ensure that provided password matches the expected password
        if (ServerState.SecurityHandler.SaltHashPassword(password.RawPassword, Salt).SequenceEqual(HashedPassword))
        {
            _ = ServerState.UserDatabase.Kill();
            if (!ServerState.UserDatabase.DoesExist())
            {
                ServerState.UserDatabase.CreateTables();
            }

            // ServerState.ResearchDatabase.Kill();
            // ServerState.ResearchDatabase.CreateTables();

            return Ok();
        }
        else
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Hashes and salts the provided password
    /// </summary>
    /// <param name="password">Password to hash and salt</param>
    /// <returns><see cref="StatusCodes.Status200OK"/>(<see cref="Common.POCOs.HashAndSalt"/>)</returns>
    [ProducesResponseType(typeof(HashAndSalt), StatusCodes.Status200OK)]
    [HttpPost("HashAndSalt", Name = "HashAndSalt")]
    public IActionResult HashAndSalt([FromBody] Password password)
    {
        (byte[] hash, byte[] salt) = ServerState.SecurityHandler.SaltHashPassword(password.RawPassword);
        return Ok(new HashAndSalt(hash, salt));
    }
}
