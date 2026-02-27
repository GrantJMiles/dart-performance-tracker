using DartPerformanceTracker.Server.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DartPerformanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TeamDto>>> GetAll() => Ok(await _teamService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetById(int id)
    {
        var team = await _teamService.GetByIdAsync(id);
        return team == null ? NotFound() : Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<TeamDto>> Create([FromBody] CreateTeamDto dto)
    {
        var created = await _teamService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
