using DartPerformanceTracker.Functions.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DartPerformanceTracker.Functions.Functions;

public class PlayersFunction(IPlayerService playerService)
{
    [Function("GetPlayers")]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "players")] HttpRequest req)
    {
        return new OkObjectResult(await playerService.GetAllAsync());
    }

    [Function("GetPlayerById")]
    public async Task<IActionResult> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "players/{id:int}")] HttpRequest req,
        int id)
    {
        var player = await playerService.GetByIdAsync(id);
        return player == null ? new NotFoundResult() : new OkObjectResult(player);
    }

    [Function("CreatePlayer")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "players")] HttpRequest req)
    {
        var dto = await req.ReadFromJsonAsync<CreatePlayerDto>();
        if (dto == null) return new BadRequestObjectResult("Invalid request body.");
        var created = await playerService.CreateAsync(dto);
        return new CreatedResult($"/api/players/{created.Id}", created);
    }
}
