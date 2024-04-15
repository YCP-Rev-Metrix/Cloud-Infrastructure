using Common.Logging;
using Common.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Controllers.APIControllers;

namespace Server.Controllers.DatabaseControllers.Posts;

[ApiController]
[Tags("Posts")]
[Route("api/posts/[controller]")]
public class InsertShotController : AbstractFeaturedController
{
    [HttpPost(Name = "InsertShot")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertShot([FromBody] Shot shot)
    {
        var result = await ServerState.UserStore.InsertShot(
            shot.User_id,
            shot.Frame_id,
            shot.Ball_id,
            shot.Video_id,
            shot.Pins_remaining,
            shot.Time,
            shot.Lane_Number,
            shot.Ddx,
            shot.Ddy,
            shot.Ddz,
            shot.X_position,
            shot.Y_position,
            shot.Z_position,
            shot.Pocket_hit
        );

        return Ok(result);
    }
}
