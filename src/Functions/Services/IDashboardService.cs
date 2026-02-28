using DartPerformanceTracker.Shared.DTOs;

namespace DartPerformanceTracker.Functions.Services;

public interface IDashboardService
{
    Task<TeamDashboardDto?> GetTeamDashboardAsync(int seasonId);
    Task<TeamDashboardDto?> GetTeamDashboardAsync(int teamId, int seasonId);
    Task<PlayerDashboardDto?> GetPlayerDashboardAsync(int playerId);
}
