using DartPerformanceTracker.Server.Data;
using DartPerformanceTracker.Shared.DTOs;
using DartPerformanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Server.Services;

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
            .Include(g => g.Matches).ThenInclude(m => m.PlayerStats)
            .Include(g => g.ManOfTheMatchAwards)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (gn == null) return null;

        return MapToDto(gn);
    }

    public async Task<GameNightDto> CreateAsync(CreateGameNightDto dto)
    {
        var gameNight = new GameNight
        {
            SeasonId = dto.SeasonId,
            Date = dto.Date,
            Opponent = dto.Opponent,
            IsHome = dto.IsHome,
            Matches = dto.Matches.Select(m => new Match
            {
                MatchTypeId = m.MatchTypeId,
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

    private static GameNightDto MapToDto(GameNight gn)
    {
        return new GameNightDto
        {
            Id = gn.Id,
            SeasonId = gn.SeasonId,
            Date = gn.Date,
            Opponent = gn.Opponent,
            IsHome = gn.IsHome,
            Matches = gn.Matches.Select(m => new MatchDto
            {
                Id = m.Id,
                GameNightId = m.GameNightId,
                MatchTypeId = m.MatchTypeId,
                MatchTypeName = m.MatchType?.Name ?? string.Empty,
                LegsWon = m.LegsWon,
                LegsLost = m.LegsLost,
                Won = m.Won,
                PlayerIds = m.MatchPlayers.Select(mp => mp.PlayerId).ToList(),
                PlayerStats = m.PlayerStats.Select(ps => new PlayerMatchStatsDto
                {
                    Id = ps.Id,
                    PlayerId = ps.PlayerId,
                    Tons = ps.Tons,
                    Maximums = ps.Maximums
                }).ToList()
            }).ToList(),
            ManOfTheMatchPlayerIds = gn.ManOfTheMatchAwards.Select(m => m.PlayerId).ToList()
        };
    }
}
