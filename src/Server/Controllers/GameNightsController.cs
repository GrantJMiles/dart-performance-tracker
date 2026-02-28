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

    [HttpGet]
    public async Task<ActionResult<List<GameNightDto>>> GetIncomplete([FromQuery] bool incomplete = false)
    {
        if (!incomplete) return BadRequest("Use ?incomplete=true");
        return Ok(await _gameNightService.GetIncompleteAsync());
    }

    [HttpPost]
    public async Task<ActionResult<GameNightDto>> Create([FromBody] CreateGameNightDto dto)
    {
        var created = await _gameNightService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id}/matches/{matchId}")]
    public async Task<ActionResult<GameNightDto>> UpdateMatch(int id, int matchId, [FromBody] UpdateMatchDto dto)
    {
        var result = await _gameNightService.UpdateMatchAsync(id, matchId, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id}/motm")]
    public async Task<ActionResult<GameNightDto>> UpdateMotm(int id, [FromBody] UpdateMotmDto dto)
    {
        var result = await _gameNightService.UpdateMotmAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }
}
