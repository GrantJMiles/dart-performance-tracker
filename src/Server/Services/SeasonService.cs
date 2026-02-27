using DartPerformanceTracker.Server.Data;
using DartPerformanceTracker.Shared.DTOs;
using DartPerformanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Server.Services;

public class SeasonService : ISeasonService
{
    private readonly AppDbContext _context;

    public SeasonService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SeasonDto>> GetAllAsync()
    {
        return await _context.Seasons
            .Include(s => s.MatchConfigurations).ThenInclude(mc => mc.MatchType)
            .Select(s => new SeasonDto
            {
                Id = s.Id,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                MatchConfigurations = s.MatchConfigurations.Select(mc => new SeasonMatchConfigurationDto
                {
                    Id = mc.Id,
                    SeasonId = mc.SeasonId,
                    MatchTypeId = mc.MatchTypeId,
                    MatchTypeName = mc.MatchType.Name,
                    PlayersPerSide = mc.MatchType.PlayersPerSide,
                    NumberOfMatches = mc.NumberOfMatches,
                    OrderIndex = mc.OrderIndex
                }).OrderBy(mc => mc.OrderIndex).ToList()
            })
            .ToListAsync();
    }

    public async Task<SeasonDto> CreateAsync(CreateSeasonDto dto)
    {
        var season = new Season
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MatchConfigurations = dto.MatchConfigurations.Select(mc => new SeasonMatchConfiguration
            {
                MatchTypeId = mc.MatchTypeId,
                NumberOfMatches = mc.NumberOfMatches,
                OrderIndex = mc.OrderIndex
            }).ToList()
        };
        _context.Seasons.Add(season);
        await _context.SaveChangesAsync();

        var created = await _context.Seasons
            .Include(s => s.MatchConfigurations).ThenInclude(mc => mc.MatchType)
            .FirstAsync(s => s.Id == season.Id);

        return new SeasonDto
        {
            Id = created.Id,
            Name = created.Name,
            StartDate = created.StartDate,
            EndDate = created.EndDate,
            MatchConfigurations = created.MatchConfigurations.Select(mc => new SeasonMatchConfigurationDto
            {
                Id = mc.Id,
                SeasonId = mc.SeasonId,
                MatchTypeId = mc.MatchTypeId,
                MatchTypeName = mc.MatchType.Name,
                PlayersPerSide = mc.MatchType.PlayersPerSide,
                NumberOfMatches = mc.NumberOfMatches,
                OrderIndex = mc.OrderIndex
            }).OrderBy(mc => mc.OrderIndex).ToList()
        };
    }
}
