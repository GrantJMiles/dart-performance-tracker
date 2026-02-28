using DartPerformanceTracker.Functions.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DartPerformanceTracker.Functions.Functions;

public class TeamsFunction(ITeamService teamService)
{
    [Function("GetTeams")]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "teams")] HttpRequest req)
    {
        return new OkObjectResult(await teamService.GetAllAsync());
    }

    [Function("GetTeamById")]
    public async Task<IActionResult> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "teams/{id:int}")] HttpRequest req,
        int id)
    {
        var team = await teamService.GetByIdAsync(id);
        return team == null ? new NotFoundResult() : new OkObjectResult(team);
    }

    [Function("CreateTeam")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "teams")] HttpRequest req)
    {
        var dto = await req.ReadFromJsonAsync<CreateTeamDto>();
        if (dto == null) return new BadRequestObjectResult("Invalid request body.");
        var created = await teamService.CreateAsync(dto);
        return new CreatedResult($"/api/teams/{created.Id}", created);
    }
}
