using Common.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;
// Authorize Refresh Register
// Logout Unregister ChangePassword

[ApiController]
[Route("api/[controller]")]
public class ResearchController : AbstractFeaturedController
{
    static Random random = new Random();
    
    
    private byte[] pins_remaining = new byte[8];
    private DateTime time = DateTime.Now;
    private byte[] lane_number = new byte[8];
    private float x = random.NextInt64();
    private float y = random.NextInt64();
    private float z = random.NextInt64();


    
    [HttpPost("Testing", Name = "Testing")]
    public async Task<IActionResult> CreateShot()
    {
        
        //ServerState.ResearchDatabase.CreateTables();
        return Ok(await ServerState.ResearchDatabase.AddShot(pins_remaining, time, lane_number, x, y, z));
    }


}
