using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Server.Services;

public interface IGameNightService
{
    Task<GameNightDto?> GetByIdAsync(int id);
    Task<GameNightDto> CreateAsync(CreateGameNightDto dto);
}
