using DartPerformanceTracker.Server.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DartPerformanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlayerDto>>> GetAll() => Ok(await _playerService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetById(int id)
    {
        var player = await _playerService.GetByIdAsync(id);
        return player == null ? NotFound() : Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create([FromBody] CreatePlayerDto dto)
    {
        var created = await _playerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
