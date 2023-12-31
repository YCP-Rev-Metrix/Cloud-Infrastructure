﻿using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResearchController : AbstractFeaturedController
{
    private static readonly Random random = new();

    private readonly byte[] pins_remaining = new byte[8];
    private readonly DateTime time = DateTime.Now;
    private readonly byte[] lane_number = new byte[8];
    private readonly float x = random.NextInt64();
    private readonly float y = random.NextInt64();
    private readonly float z = random.NextInt64();

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [HttpPost("Testing", Name = "Testing")]
    public async Task<IActionResult> CreateShot() =>

        //ServerState.ResearchDatabase.CreateTables();
        Ok(await ServerState.ResearchDatabase.AddShot(pins_remaining, time, lane_number, x, y, z));

}
