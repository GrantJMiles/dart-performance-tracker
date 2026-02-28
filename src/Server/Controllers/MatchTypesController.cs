using DartPerformanceTracker.Server.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DartPerformanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchTypesController : ControllerBase
{
    private readonly IMatchTypeService _matchTypeService;

    public MatchTypesController(IMatchTypeService matchTypeService)
    {
        _matchTypeService = matchTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MatchTypeDto>>> GetAll()
    {
        return Ok(await _matchTypeService.GetAllAsync());
    }
}
