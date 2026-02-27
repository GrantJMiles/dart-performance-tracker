using DartPerformanceTracker.Server.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DartPerformanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameNightsController : ControllerBase
{
    private readonly IGameNightService _gameNightService;

    public GameNightsController(IGameNightService gameNightService)
    {
        _gameNightService = gameNightService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameNightDto>> GetById(int id)
    {
        var gn = await _gameNightService.GetByIdAsync(id);
        return gn == null ? NotFound() : Ok(gn);
    }

    [HttpPost]
    public async Task<ActionResult<GameNightDto>> Create([FromBody] CreateGameNightDto dto)
    {
        var created = await _gameNightService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
