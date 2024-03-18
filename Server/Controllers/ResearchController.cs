using Common.POCOs;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResearchController : AbstractFeaturedController
{

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [HttpPost("InsertVideo", Name = "InsertVideo")]
    public async Task<IActionResult> CreateVideo([FromBody] Video video)
    {
        return Ok(await ServerState.ResearchDatabase.AddVideo(video.Video_file, video.Video_id));
    }

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [HttpPost("Insertddx", Name = "Insertddx")]
    public async Task<IActionResult> CreateDdx([FromBody] ddx ddx)
    {
        return Ok(await ServerState.ResearchDatabase.AddDdx(ddx.Accelerations));
    }

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [HttpPost("Insertddy", Name = "Insertddy")]
    public async Task<IActionResult> CreateDdy([FromBody] ddy ddy)
    {
        return Ok(await ServerState.ResearchDatabase.AddDdy(ddy.Accelerations));
    }

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [HttpPost("Insertddz", Name = "Insertddz")]
    public async Task<IActionResult> CreateDdz([FromBody] ddz ddz)
    {
        return Ok(await ServerState.ResearchDatabase.AddDdz(ddz.Accelerations));
    }

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [HttpPost("InsertLight", Name = "InsertLight")]
    public async Task<IActionResult> CreateLight([FromBody] Light light)
    {
        return Ok(await ServerState.ResearchDatabase.AddLight(light.Light_levels));
    }

     //TODO:GET Endpionts, Data -> Date -> Session? -> -> User 
     [HttpGet("GetShot", Name = "GetShot")]
     [ProducesResponseType(typeof(Shot), StatusCodes.Status200OK)]
     public async Task<IActionResult>  GetShot([FromBody] UserIdentification user)
     {
         return Ok(await ServerState.ResearchDatabase.GetShotData(user.Username));
     }




}
