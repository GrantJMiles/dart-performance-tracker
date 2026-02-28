using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Functions.Services;

public interface IGameNightService
{
    Task<GameNightDto?> GetByIdAsync(int id);
    Task<List<GameNightDto>> GetIncompleteAsync();
    Task<GameNightDto> CreateAsync(CreateGameNightDto dto);
    Task<GameNightDto?> UpdateMatchAsync(int gameNightId, int matchId, UpdateMatchDto dto);
    Task<GameNightDto?> UpdateMotmAsync(int gameNightId, UpdateMotmDto dto);
}
