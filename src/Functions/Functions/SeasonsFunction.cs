using DartPerformanceTracker.Functions.Services;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DartPerformanceTracker.Functions.Functions;

public class SeasonsFunction(ISeasonService seasonService)
{
    [Function("GetSeasons")]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "seasons")] HttpRequest req)
    {
        return new OkObjectResult(await seasonService.GetAllAsync());
    }

    [Function("CreateSeason")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "seasons")] HttpRequest req)
    {
        var dto = await req.ReadFromJsonAsync<CreateSeasonDto>();
        if (dto == null) return new BadRequestObjectResult("Invalid request body.");
        var created = await seasonService.CreateAsync(dto);
        return new CreatedResult($"/api/seasons/{created.Id}", created);
    }
}
