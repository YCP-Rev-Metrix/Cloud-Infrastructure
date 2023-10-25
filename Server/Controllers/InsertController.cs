using Common.POCOs;
using DatabaseCore;
using Microsoft.AspNetCore.Mvc;
using Server;
using Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InsertController : ControllerBase
{
    private readonly UserDB _userDB;
    private readonly ResearchDB _researchDB;

    public InsertController(UserDB userDB) => _userDB = userDB;
   
    [HttpPost("Insert", Name = "Insert")]
    public async Task<IActionResult> Insert([FromBody] InsertUserData userIdentification)
    {
        // Validate user credentials (e.g., check against a database)
        bool result = await ServerState.UserStore.InsertUser(userIdentification.Userid, 
                                                            userIdentification.Firstname, 
                                                            userIdentification.Lastname ,
                                                            userIdentification.Username,
                                                            userIdentification.Password, 
                                                            userIdentification.Email, 
                                                            userIdentification.Phone, 
                                                            userIdentification.Role);
   
        return new JsonResult(result);
    }
}
    