using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Controller]
public abstract class AbstractFeaturedController : ControllerBase
{
    [NonAction]
    public string? GetUsername() => HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

    [NonAction]
    public string? GetJWT() => HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
}
