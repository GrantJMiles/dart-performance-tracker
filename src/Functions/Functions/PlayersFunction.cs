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

    [Function("UpdatePlayerStatus")]
    public async Task<IActionResult> UpdateStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "players/{id:int}/status")] HttpRequest req,
        int id)
    {
        var dto = await req.ReadFromJsonAsync<UpdatePlayerStatusDto>();
        if (dto == null) return new BadRequestObjectResult("Invalid request body.");
        var updated = await playerService.UpdateStatusAsync(id, dto.IsActive);
        return updated == null ? new NotFoundResult() : new OkObjectResult(updated);
    }
}
