using DartPerformanceTracker.Data;
using DartPerformanceTracker.Shared.DTOs;
using DartPerformanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Functions.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TeamDashboardDto?> GetTeamDashboardAsync(int seasonId)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) return null;

        var gameNights = await _context.GameNights
            .Include(g => g.Matches)
            .Where(g => g.SeasonId == seasonId)
            .ToListAsync();

        return BuildDashboard(season.Name, seasonId, gameNights);
    }

    public async Task<TeamDashboardDto?> GetTeamDashboardAsync(int teamId, int seasonId)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) return null;

        // Only include game nights where at least one player from the team participated
        var gameNightIds = await _context.MatchPlayers
            .Where(mp => mp.Player.TeamId == teamId && mp.Match.GameNight.SeasonId == seasonId)
            .Select(mp => mp.Match.GameNightId)
            .Distinct()
            .ToListAsync();

        var gameNights = await _context.GameNights
            .Include(g => g.Matches)
            .Where(g => gameNightIds.Contains(g.Id))
            .ToListAsync();

        return BuildDashboard(season.Name, seasonId, gameNights);
    }

    private static TeamDashboardDto BuildDashboard(string seasonName, int seasonId, List<GameNight> gameNights)
    {
        var summaries = gameNights.Select(g =>
        {
            var won = g.Matches.Count(m => m.Won);
            var lost = g.Matches.Count(m => !m.Won);
            return new GameNightSummaryDto
            {
                GameNightId = g.Id,
                Date = g.Date,
                Opponent = g.Opponent,
                IsHome = g.IsHome,
                MatchesWon = won,
                MatchesLost = lost,
                Won = won > lost
            };
        }).ToList();

        var orderedSummaries = summaries.OrderByDescending(s => s.Date).ToList();

        var lastFiveForm = orderedSummaries
            .Take(5)
            .Select(s => s.MatchesWon == s.MatchesLost ? "D" : s.Won ? "W" : "L")
            .ToList();

        return new TeamDashboardDto
        {
            SeasonId = seasonId,
            SeasonName = seasonName,
            TotalGameNights = gameNights.Count,
            GameNightsWon = summaries.Count(s => s.Won),
            GameNightsLost = summaries.Count(s => !s.Won && s.MatchesWon != s.MatchesLost),
            GameNightsDrawn = summaries.Count(s => s.MatchesWon == s.MatchesLost),
            TotalMatchesPlayed = gameNights.Sum(g => g.Matches.Count),
            TotalMatchesWon = gameNights.Sum(g => g.Matches.Count(m => m.Won)),
            TotalMatchesLost = gameNights.Sum(g => g.Matches.Count(m => !m.Won)),
            GameNightSummaries = orderedSummaries,
            LastFiveForm = lastFiveForm
        };
    }

    public async Task<PlayerDashboardDto?> GetPlayerDashboardAsync(int playerId)
    {
        var player = await _context.Players.Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == playerId);
        if (player == null) return null;

        var matchPlayers = await _context.MatchPlayers
            .Include(mp => mp.Match).ThenInclude(m => m.GameNight)
            .Include(mp => mp.Match).ThenInclude(m => m.MatchType)
            .Where(mp => mp.PlayerId == playerId)
            .ToListAsync();

        var stats = await _context.PlayerMatchStats
            .Where(ps => ps.PlayerId == playerId)
            .ToListAsync();

        var motmCount = await _context.ManOfTheMatches.CountAsync(m => m.PlayerId == playerId);

        // Top teammate: find the co-player who has shared the most matches, ordered by wins together
        var matchIds = matchPlayers.Select(mp => mp.MatchId).ToHashSet();
        var teammateStats = await _context.MatchPlayers
            .Where(mp => matchIds.Contains(mp.MatchId) && mp.PlayerId != playerId)
            .Select(mp => new { mp.PlayerId, mp.Player.Name, mp.Match.Won })
            .ToListAsync();

        TopTeammateDto? topTeammate = null;
        if (teammateStats.Count > 0)
        {
            var grouped = teammateStats
                .GroupBy(mp => mp.PlayerId)
                .Select(g => new
                {
                    PlayerId = g.Key,
                    PlayerName = g.First().Name,
                    GamesPlayedTogether = g.Count(),
                    WinsTogether = g.Count(mp => mp.Won)
                })
                .OrderByDescending(t => t.GamesPlayedTogether)
                .ThenByDescending(t => t.WinsTogether)
                .FirstOrDefault();

            if (grouped != null)
            {
                topTeammate = new TopTeammateDto
                {
                    PlayerId = grouped.PlayerId,
                    PlayerName = grouped.PlayerName,
                    GamesPlayedTogether = grouped.GamesPlayedTogether,
                    WinsTogether = grouped.WinsTogether
                };
            }
        }

        var recentMatches = matchPlayers.Select(mp =>
        {
            var playerStats = stats.FirstOrDefault(s => s.MatchId == mp.MatchId);
            return new PlayerMatchResultDto
            {
                MatchId = mp.MatchId,
                Date = mp.Match.GameNight.Date,
                Opponent = mp.Match.GameNight.Opponent,
                MatchTypeName = mp.Match.MatchType?.Name ?? string.Empty,
                Won = mp.Match.Won,
                Tons = playerStats?.Tons ?? 0,
                Maximums = playerStats?.Maximums ?? 0
            };
        }).OrderByDescending(r => r.Date).Take(20).ToList();

        return new PlayerDashboardDto
        {
            PlayerId = playerId,
            PlayerName = player.Name,
            TeamName = player.Team?.Name ?? string.Empty,
            MatchesPlayed = matchPlayers.Count,
            MatchesWon = matchPlayers.Count(mp => mp.Match.Won),
            MatchesLost = matchPlayers.Count(mp => !mp.Match.Won),
            TotalTons = stats.Sum(s => s.Tons),
            TotalMaximums = stats.Sum(s => s.Maximums),
            ManOfTheMatchCount = motmCount,
            TopTeammate = topTeammate,
            RecentMatches = recentMatches
        };
    }
}
