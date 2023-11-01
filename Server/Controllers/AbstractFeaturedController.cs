using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

public abstract class AbstractFeaturedController : ControllerBase
{
    public string? GetUsername() => HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

    public string? GetJWT() => HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
}
