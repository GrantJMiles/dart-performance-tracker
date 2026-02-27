using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Server.Services;

public interface ISeasonService
{
    Task<List<SeasonDto>> GetAllAsync();
    Task<SeasonDto> CreateAsync(CreateSeasonDto dto);
}
