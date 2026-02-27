using DartPerformanceTracker.Server.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DartPerformanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("team/{seasonId}")]
    public async Task<ActionResult<TeamDashboardDto>> GetTeamDashboard(int seasonId)
    {
        var result = await _dashboardService.GetTeamDashboardAsync(seasonId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("team/{teamId}/season/{seasonId}")]
    public async Task<ActionResult<TeamDashboardDto>> GetTeamSeasonDashboard(int teamId, int seasonId)
    {
        var result = await _dashboardService.GetTeamDashboardAsync(teamId, seasonId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("player/{playerId}")]
    public async Task<ActionResult<PlayerDashboardDto>> GetPlayerDashboard(int playerId)
    {
        var result = await _dashboardService.GetPlayerDashboardAsync(playerId);
        return result == null ? NotFound() : Ok(result);
    }
}
