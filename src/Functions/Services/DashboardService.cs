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
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
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
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
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
                Won = won > lost,
                TotalTons = g.Matches.Sum(m => m.PlayerStats.Sum(ps => ps.Tons)),
                TotalMaximums = g.Matches.Sum(m => m.PlayerStats.Sum(ps => ps.Maximums))
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

    public async Task<DashboardInsightsDto?> GetDashboardInsightsAsync(int teamId, int seasonId)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) return null;

        // Get all match players for this team in this season, with match type info
        var matchPlayers = await _context.MatchPlayers
            .Include(mp => mp.Player)
            .Include(mp => mp.Match).ThenInclude(m => m.MatchType)
            .Include(mp => mp.Match).ThenInclude(m => m.GameNight)
            .Where(mp => mp.Player.TeamId == teamId && mp.Match.GameNight.SeasonId == seasonId)
            .ToListAsync();

        // Singles MVP: players in singles matches (PlayersPerSide == 1), min 5 games, best win%
        var singlesPlayers = matchPlayers
            .Where(mp => mp.Match.MatchType.PlayersPerSide == 1)
            .GroupBy(mp => mp.PlayerId)
            .Where(g => g.Count() >= 5)
            .Select(g => new MvpDto
            {
                PlayerId = g.Key,
                PlayerName = g.First().Player.Name,
                MatchesPlayed = g.Count(),
                MatchesWon = g.Count(mp => mp.Match.Won),
                WinPercentage = g.Count() > 0 ? Math.Round((double)g.Count(mp => mp.Match.Won) / g.Count() * 100, 1) : 0
            })
            .OrderByDescending(p => p.WinPercentage)
            .ThenByDescending(p => p.MatchesWon)
            .FirstOrDefault();

        // Season MVP: most wins across singles + pairs (PlayersPerSide <= 2)
        var seasonMvp = matchPlayers
            .Where(mp => mp.Match.MatchType.PlayersPerSide <= 2)
            .GroupBy(mp => mp.PlayerId)
            .Select(g => new MvpDto
            {
                PlayerId = g.Key,
                PlayerName = g.First().Player.Name,
                MatchesPlayed = g.Count(),
                MatchesWon = g.Count(mp => mp.Match.Won),
                WinPercentage = g.Count() > 0 ? Math.Round((double)g.Count(mp => mp.Match.Won) / g.Count() * 100, 1) : 0
            })
            .OrderByDescending(p => p.MatchesWon)
            .ThenByDescending(p => p.WinPercentage)
            .FirstOrDefault();

        // Pairs MVP: pair with most wins in pairs matches (PlayersPerSide == 2)
        var pairsMatches = matchPlayers
            .Where(mp => mp.Match.MatchType.PlayersPerSide == 2)
            .GroupBy(mp => mp.MatchId)
            .Where(g => g.Count() == 2)
            .ToList();

        PairsMvpDto? pairsMvp = null;
        if (pairsMatches.Count > 0)
        {
            var pairStats = pairsMatches
                .Select(g =>
                {
                    var players = g.OrderBy(mp => mp.PlayerId).ToList();
                    return new
                    {
                        Key = $"{players[0].PlayerId}_{players[1].PlayerId}",
                        Player1Name = players[0].Player.Name,
                        Player2Name = players[1].Player.Name,
                        Won = g.First().Match.Won
                    };
                })
                .GroupBy(x => x.Key)
                .Select(g => new PairsMvpDto
                {
                    Player1Name = g.First().Player1Name,
                    Player2Name = g.First().Player2Name,
                    MatchesPlayed = g.Count(),
                    MatchesWon = g.Count(x => x.Won)
                })
                .OrderByDescending(p => p.MatchesWon)
                .ThenByDescending(p => p.MatchesPlayed)
                .FirstOrDefault();

            pairsMvp = pairStats;
        }

        return new DashboardInsightsDto
        {
            SinglesMvp = singlesPlayers,
            SeasonMvp = seasonMvp,
            PairsMvp = pairsMvp
        };
    }

    public async Task<List<TeamPlayerSeasonStatsDto>> GetTeamPlayerStatsAsync(int teamId, int seasonId)
    {
        var matchTypeOrder = await _context.SeasonMatchConfigurations
            .Where(c => c.SeasonId == seasonId)
            .ToDictionaryAsync(c => c.MatchTypeId, c => c.OrderIndex);

        var matchPlayers = await _context.MatchPlayers
            .Include(mp => mp.Player)
            .Include(mp => mp.Match).ThenInclude(m => m.MatchType)
            .Include(mp => mp.Match).ThenInclude(m => m.GameNight)
            .Include(mp => mp.Match).ThenInclude(m => m.PlayerStats)
            .Where(mp => mp.Player.TeamId == teamId && mp.Match.GameNight.SeasonId == seasonId)
            .ToListAsync();

        var motmCounts = await _context.ManOfTheMatches
            .Where(m => m.GameNight.SeasonId == seasonId && m.Player.TeamId == teamId)
            .GroupBy(m => m.PlayerId)
            .Select(g => new { PlayerId = g.Key, Count = g.Count() })
            .ToListAsync();

        var motmLookup = motmCounts.ToDictionary(x => x.PlayerId, x => x.Count);

        var result = matchPlayers
            .GroupBy(mp => mp.PlayerId)
            .Select(playerGroup =>
            {
                var playerId = playerGroup.Key;
                var playerName = playerGroup.First().Player.Name;

                var matchTypeBreakdowns = playerGroup
                    .GroupBy(mp => mp.Match.MatchTypeId)
                    .Select(typeGroup =>
                    {
                        var matchTypeId = typeGroup.Key;
                        var playerStats = typeGroup
                            .SelectMany(mp => mp.Match.PlayerStats.Where(ps => ps.PlayerId == playerId))
                            .ToList();
                        return new
                        {
                            Order = matchTypeOrder.TryGetValue(matchTypeId, out var order) ? order : int.MaxValue,
                            Breakdown = new PlayerMatchTypeBreakdownDto
                            {
                                MatchTypeName = typeGroup.First().Match.MatchType.Name,
                                MatchesPlayed = typeGroup.Count(),
                                MatchesWon = typeGroup.Count(mp => mp.Match.Won),
                                LegsWon = typeGroup.Sum(mp => mp.Match.LegsWon),
                                LegsLost = typeGroup.Sum(mp => mp.Match.LegsLost),
                                Tons = playerStats.Sum(ps => ps.Tons),
                                Maximums = playerStats.Sum(ps => ps.Maximums)
                            }
                        };
                    })
                    .OrderBy(x => x.Order)
                    .Select(x => x.Breakdown)
                    .ToList();

                var matchIds = playerGroup.Select(mp => mp.MatchId).ToHashSet();
                var coPlayers = matchPlayers
                    .Where(mp => matchIds.Contains(mp.MatchId) && mp.PlayerId != playerId)
                    .ToList();

                var topPairings = coPlayers
                    .GroupBy(mp => mp.PlayerId)
                    .Select(g =>
                    {
                        var played = g.Count();
                        var won = g.Count(mp => mp.Match.Won);
                        return new TopPairingDto
                        {
                            PartnerName = g.First().Player.Name,
                            MatchesPlayed = played,
                            MatchesWon = won,
                            WinRatio = played > 0 ? Math.Round((double)won / played * 100, 1) : 0
                        };
                    })
                    .OrderByDescending(p => p.WinRatio)
                    .ThenByDescending(p => p.MatchesWon)
                    .ToList();

                return new TeamPlayerSeasonStatsDto
                {
                    PlayerId = playerId,
                    PlayerName = playerName,
                    ManOfTheMatchCount = motmLookup.GetValueOrDefault(playerId, 0),
                    MatchTypeBreakdowns = matchTypeBreakdowns,
                    TopPairings = topPairings
                };
            })
            .OrderBy(p => p.PlayerName)
            .ToList();

        return result;
    }
}
