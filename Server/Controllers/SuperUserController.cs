using Common.POCOs;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

public class SuperUserController : AbstractFeaturedController
{
    private readonly byte[] Salt = Convert.FromBase64String("tu99J/MoR/0fPqiANiUSsQ==");
    private readonly byte[] HashedPassword = Convert.FromBase64String("f389lt8C+LGKL8x02bqt3QKP+FUFMdPchLesmSeHgMY=");

    [HttpPost("ByeBye", Name = "ByeBye")]
    public IActionResult ByeBye([FromBody] Password password)
    {
        if (ServerState.SecurityHandler.SaltHashPassword(password.RawPassword, Salt).SequenceEqual(HashedPassword))
        {
            ServerState.UserDatabase.Kill();
            ServerState.UserDatabase.CreateTables();
            ServerState.UserDatabase.Initialize();

            ServerState.ResearchDatabase.Kill();
            ServerState.ResearchDatabase.CreateTables();
            ServerState.ResearchDatabase.Initialize();
           
            return Ok();
        }
        else
        {
            return Forbid();
        }
    }

    [HttpPost("HashAndSalt", Name = "HashAndSalt")]
    public IActionResult HashAndSalt([FromBody] Password password)
    {
        (byte[] hash, byte[] salt) = ServerState.SecurityHandler.SaltHashPassword(password.RawPassword);
        return Ok(new HashAndSalt(hash, salt));
    }
}
