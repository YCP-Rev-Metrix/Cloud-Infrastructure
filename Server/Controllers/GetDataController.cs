using Common.POCOs;
using DatabaseCore;
using Microsoft.AspNetCore.Mvc;
using Server;
using Server.Controllers;



namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GetDataController : ControllerBase
{

    [HttpPost("GetData", Name = "GetData")]
    public async Task<IActionResult> GetData([FromBody] InsertUserData userIdentification)
    {
        // return the username from the db where you give the user id
        (bool success, string? result) = await ServerState.UserStore.GetUsername( userIdentification.Userid);
        // return as a jsonresult
        return new JsonResult(result);
    }
}
    