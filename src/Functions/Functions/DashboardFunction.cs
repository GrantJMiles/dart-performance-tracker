using DartPerformanceTracker.Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DartPerformanceTracker.Functions.Functions;

public class DashboardFunction(IDashboardService dashboardService)
{
    [Function("GetTeamDashboard")]
    public async Task<IActionResult> GetTeamDashboard(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboard/team/{seasonId:int}")] HttpRequest req,
        int seasonId)
    {
        var result = await dashboardService.GetTeamDashboardAsync(seasonId);
        return result == null ? new NotFoundResult() : new OkObjectResult(result);
    }

    [Function("GetTeamSeasonDashboard")]
    public async Task<IActionResult> GetTeamSeasonDashboard(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboard/team/{teamId:int}/season/{seasonId:int}")] HttpRequest req,
        int teamId, int seasonId)
    {
        var result = await dashboardService.GetTeamDashboardAsync(teamId, seasonId);
        return result == null ? new NotFoundResult() : new OkObjectResult(result);
    }

    [Function("GetPlayerDashboard")]
    public async Task<IActionResult> GetPlayerDashboard(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboard/player/{playerId:int}")] HttpRequest req,
        int playerId)
    {
        var result = await dashboardService.GetPlayerDashboardAsync(playerId);
        return result == null ? new NotFoundResult() : new OkObjectResult(result);
    }
}
