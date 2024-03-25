using Common.Logging;
using Common.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Controllers.APIControllers;

namespace Server.Controllers.DatabaseControllers.Deletes;

[ApiController]
[Route("api/[controller]")]
public class StartEventController : AbstractFeaturedController
{
    [HttpPost(Name = "StartEvent")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartEvent([FromBody] Event events)
    {
        return Ok(await ServerState.UserStore.StartEvent(events.User_id, events.Event_type));
    }
}
