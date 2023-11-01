using Microsoft.AspNetCore.Authorization;

namespace Server.Middleware;

public class VerifyJWTBlacklistMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Endpoint? endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            if (endpoint.Metadata.GetMetadata<AuthorizeAttribute>() != null)
            {
                string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    if (!ServerState.TokenStore.IsAuthorizationBlacklisted(token))
                    {
                        await next.Invoke(context);
                        return;
                    }
                }

                context.Response.StatusCode = 401;
                return;
            }
        }

        await next.Invoke(context);
    }
}
