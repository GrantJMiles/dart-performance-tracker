using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Server.Services;

public interface IPlayerService
{
    Task<List<PlayerDto>> GetAllAsync();
    Task<PlayerDto?> GetByIdAsync(int id);
    Task<PlayerDto> CreateAsync(CreatePlayerDto dto);
}
