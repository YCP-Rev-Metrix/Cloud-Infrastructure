using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Server;
using Server.Middleware;

internal class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        //_ = builder.WebHost.UseSetting("https_port", "443");

        _ = builder.Services.AddScoped<VerifyJWTBlacklistMiddleware>();

        _ = builder.Logging.ClearProviders();
        _ = builder.Logging.AddConsole();
        // Add services to the container.

        _ = builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        _ = builder.Services.AddEndpointsApiExplorer();
        _ = builder.Services.AddSwaggerGen();

        _ = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = Config.AuthIssuer,
                ValidAudience = Config.AuthAudience,
                IssuerSigningKey = ServerState.SecurityHandler.AuthorizationSigningTokenKey,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        // Verify token not blacklisted
        _ = app.UseMiddleware<VerifyJWTBlacklistMiddleware>();

        _ = app.UseAuthorization();

        _ = app.MapControllers();

        app.Run();
    }
}