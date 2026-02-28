using DartPerformanceTracker.Data;
using DartPerformanceTracker.Shared.DTOs;
using DartPerformanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Functions.Services;

public class GameNightService : IGameNightService
{
    private readonly AppDbContext _context;

    public GameNightService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GameNightDto?> GetByIdAsync(int id)
    {
        var gn = await _context.GameNights
            .Include(g => g.Matches).ThenInclude(m => m.MatchType)
            .Include(g => g.Matches).ThenInclude(m => m.MatchPlayers)
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats).ThenInclude(ps => ps.Player)
            .Include(g => g.ManOfTheMatchAwards)
            .FirstOrDefaultAsync(g => g.Id == id);

        return gn == null ? null : MapToDto(gn);
    }

    public async Task<List<GameNightDto>> GetIncompleteAsync()
    {
        var nights = await _context.GameNights
            .Where(gn => !gn.IsComplete)
            .Include(g => g.Matches).ThenInclude(m => m.MatchType)
            .Include(g => g.Matches).ThenInclude(m => m.MatchPlayers)
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
            .Include(g => g.ManOfTheMatchAwards)
            .OrderByDescending(gn => gn.Date)
            .ToListAsync();

        return nights.Select(MapToDto).ToList();
    }

    public async Task<GameNightDto> CreateAsync(CreateGameNightDto dto)
    {
        var gameNight = new GameNight
        {
            SeasonId = dto.SeasonId,
            Date = dto.Date,
            Opponent = dto.Opponent,
            IsHome = dto.IsHome,
            IsComplete = false,
            Matches = dto.Matches.Select(m => new Match
            {
                MatchTypeId = m.MatchTypeId,
                OrderIndex = m.OrderIndex,
                LegsWon = m.LegsWon,
                LegsLost = m.LegsLost,
                Won = m.Won,
                MatchPlayers = m.PlayerIds.Select(pid => new MatchPlayer { PlayerId = pid }).ToList(),
                PlayerStats = m.PlayerStats.Select(ps => new PlayerMatchStats
                {
                    PlayerId = ps.PlayerId,
                    Tons = ps.Tons,
                    Maximums = ps.Maximums
                }).ToList()
            }).ToList(),
            ManOfTheMatchAwards = dto.ManOfTheMatchPlayerIds.Select(pid => new ManOfTheMatch { PlayerId = pid }).ToList()
        };

        _context.GameNights.Add(gameNight);
        await _context.SaveChangesAsync();

        var created = await _context.GameNights
            .Include(g => g.Matches).ThenInclude(m => m.MatchType)
            .Include(g => g.Matches).ThenInclude(m => m.MatchPlayers)
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
            .Include(g => g.ManOfTheMatchAwards)
            .FirstAsync(g => g.Id == gameNight.Id);

        return MapToDto(created);
    }

    public async Task<GameNightDto?> UpdateMatchAsync(int gameNightId, int matchId, UpdateMatchDto dto)
    {
        var gn = await _context.GameNights
            .Include(g => g.Matches).ThenInclude(m => m.MatchPlayers)
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
            .FirstOrDefaultAsync(g => g.Id == gameNightId);

        if (gn == null) return null;

        var match = gn.Matches.FirstOrDefault(m => m.Id == matchId);
        if (match == null) return null;

        match.LegsWon = dto.LegsWon;
        match.LegsLost = dto.LegsLost;
        match.Won = dto.LegsWon > dto.LegsLost;

        _context.MatchPlayers.RemoveRange(match.MatchPlayers);
        match.MatchPlayers = dto.PlayerIds
            .Select(pid => new MatchPlayer { MatchId = matchId, PlayerId = pid })
            .ToList();

        _context.PlayerMatchStats.RemoveRange(match.PlayerStats);
        match.PlayerStats = dto.PlayerStats
            .Select(ps => new PlayerMatchStats
            {
                MatchId = matchId,
                PlayerId = ps.PlayerId,
                Tons = ps.Tons,
                Maximums = ps.Maximums
            })
            .ToList();

        await _context.SaveChangesAsync();

        var updated = await _context.GameNights
            .Include(g => g.Matches).ThenInclude(m => m.MatchType)
            .Include(g => g.Matches).ThenInclude(m => m.MatchPlayers)
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
            .Include(g => g.ManOfTheMatchAwards)
            .FirstAsync(g => g.Id == gameNightId);

        return MapToDto(updated);
    }

    public async Task<GameNightDto?> UpdateMotmAsync(int gameNightId, UpdateMotmDto dto)
    {
        var gn = await _context.GameNights
            .Include(g => g.ManOfTheMatchAwards)
            .Include(g => g.Matches).ThenInclude(m => m.MatchType)
            .Include(g => g.Matches).ThenInclude(m => m.MatchPlayers)
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
            .FirstOrDefaultAsync(g => g.Id == gameNightId);

        if (gn == null) return null;

        _context.ManOfTheMatches.RemoveRange(gn.ManOfTheMatchAwards);
        gn.ManOfTheMatchAwards = dto.PlayerIds
            .Select(pid => new ManOfTheMatch { GameNightId = gameNightId, PlayerId = pid })
            .ToList();
        gn.IsComplete = true;

        await _context.SaveChangesAsync();

        var updated = await _context.GameNights
            .Include(g => g.Matches).ThenInclude(m => m.MatchType)
            .Include(g => g.Matches).ThenInclude(m => m.MatchPlayers)
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
            .Include(g => g.ManOfTheMatchAwards)
            .FirstAsync(g => g.Id == gameNightId);

        return MapToDto(updated);
    }

    private static GameNightDto MapToDto(GameNight gn)
    {
        return new GameNightDto
        {
            Id = gn.Id,
            SeasonId = gn.SeasonId,
            Date = gn.Date,
            Opponent = gn.Opponent,
            IsHome = gn.IsHome,
            IsComplete = gn.IsComplete,
            Matches = gn.Matches.OrderBy(m => m.OrderIndex).Select(m => new MatchDto
            {
                Id = m.Id,
                GameNightId = m.GameNightId,
                MatchTypeId = m.MatchTypeId,
                MatchTypeName = m.MatchType?.Name ?? string.Empty,
                PlayersPerSide = m.MatchType?.PlayersPerSide ?? 1,
                LegsWon = m.LegsWon,
                LegsLost = m.LegsLost,
                Won = m.Won,
                OrderIndex = m.OrderIndex,
                PlayerIds = m.MatchPlayers.Select(mp => mp.PlayerId).ToList(),
                PlayerStats = m.PlayerStats.Select(ps => new PlayerMatchStatsDto
                {
                    Id = ps.Id,
                    PlayerId = ps.PlayerId,
                    PlayerName = ps.Player?.Name ?? string.Empty,
                    Tons = ps.Tons,
                    Maximums = ps.Maximums
                }).ToList()
            }).ToList(),
            ManOfTheMatchPlayerIds = gn.ManOfTheMatchAwards.Select(m => m.PlayerId).ToList()
        };
    }
}
