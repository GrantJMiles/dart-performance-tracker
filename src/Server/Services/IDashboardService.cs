using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Server.Services;

public interface IDashboardService
{
    Task<TeamDashboardDto?> GetTeamDashboardAsync(int seasonId);
    Task<PlayerDashboardDto?> GetPlayerDashboardAsync(int playerId);
}
