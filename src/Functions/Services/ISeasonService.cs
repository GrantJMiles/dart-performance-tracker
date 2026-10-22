using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Functions.Services;

public interface ISeasonService
{
    Task<List<SeasonDto>> GetAllAsync();
    Task<SeasonDto> CreateAsync(CreateSeasonDto dto);
    Task<SeasonDto?> UpdateAsync(int id, CreateSeasonDto dto);
}
