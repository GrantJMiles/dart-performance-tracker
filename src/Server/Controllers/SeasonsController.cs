using DartPerformanceTracker.Server.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DartPerformanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeasonsController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public SeasonsController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SeasonDto>>> GetAll() => Ok(await _seasonService.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<SeasonDto>> Create([FromBody] CreateSeasonDto dto)
    {
        var created = await _seasonService.CreateAsync(dto);
        return Created($"api/seasons/{created.Id}", created);
    }
}
