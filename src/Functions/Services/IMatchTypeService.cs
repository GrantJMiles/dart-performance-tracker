using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Functions.Services;

public interface IMatchTypeService
{
    Task<List<MatchTypeDto>> GetAllAsync();
}
