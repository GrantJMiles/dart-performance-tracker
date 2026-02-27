using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Server.Services;

public interface ITeamService
{
    Task<List<TeamDto>> GetAllAsync();
    Task<TeamDto?> GetByIdAsync(int id);
    Task<TeamDto> CreateAsync(CreateTeamDto dto);
}
