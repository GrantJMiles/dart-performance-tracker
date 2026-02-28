using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Server.Services;

public interface IMatchTypeService
{
    Task<List<MatchTypeDto>> GetAllAsync();
}
