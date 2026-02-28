using DartPerformanceTracker.Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DartPerformanceTracker.Functions.Functions;

public class MatchTypesFunction(IMatchTypeService matchTypeService)
{
    [Function("GetMatchTypes")]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "matchtypes")] HttpRequest req)
    {
        return new OkObjectResult(await matchTypeService.GetAllAsync());
    }
}
