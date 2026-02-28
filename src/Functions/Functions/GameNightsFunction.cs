using DartPerformanceTracker.Functions.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DartPerformanceTracker.Functions.Functions;

public class GameNightsFunction(IGameNightService gameNightService)
{
    [Function("GetGameNightById")]
    public async Task<IActionResult> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "gamenights/{id:int}")] HttpRequest req,
        int id)
    {
        var gn = await gameNightService.GetByIdAsync(id);
        return gn == null ? new NotFoundResult() : new OkObjectResult(gn);
    }

    [Function("GetIncompleteGameNights")]
    public async Task<IActionResult> GetIncomplete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "gamenights")] HttpRequest req)
    {
        if (req.Query["incomplete"] != "true")
            return new BadRequestObjectResult("Use ?incomplete=true");
        return new OkObjectResult(await gameNightService.GetIncompleteAsync());
    }

    [Function("CreateGameNight")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "gamenights")] HttpRequest req)
    {
        var dto = await req.ReadFromJsonAsync<CreateGameNightDto>();
        if (dto == null) return new BadRequestObjectResult("Invalid request body.");
        var created = await gameNightService.CreateAsync(dto);
        return new CreatedResult($"/api/gamenights/{created.Id}", created);
    }

    [Function("UpdateGameNightMatch")]
    public async Task<IActionResult> UpdateMatch(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "gamenights/{id:int}/matches/{matchId:int}")] HttpRequest req,
        int id, int matchId)
    {
        var dto = await req.ReadFromJsonAsync<UpdateMatchDto>();
        if (dto == null) return new BadRequestObjectResult("Invalid request body.");
        var result = await gameNightService.UpdateMatchAsync(id, matchId, dto);
        return result == null ? new NotFoundResult() : new OkObjectResult(result);
    }

    [Function("UpdateGameNightMotm")]
    public async Task<IActionResult> UpdateMotm(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "gamenights/{id:int}/motm")] HttpRequest req,
        int id)
    {
        var dto = await req.ReadFromJsonAsync<UpdateMotmDto>();
        if (dto == null) return new BadRequestObjectResult("Invalid request body.");
        var result = await gameNightService.UpdateMotmAsync(id, dto);
        return result == null ? new NotFoundResult() : new OkObjectResult(result);
    }
}
