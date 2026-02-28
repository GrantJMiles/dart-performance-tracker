using DartPerformanceTracker.Data;
using DartPerformanceTracker.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Functions.Services;

public class MatchTypeService : IMatchTypeService
{
    private readonly AppDbContext _context;

    public MatchTypeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MatchTypeDto>> GetAllAsync()
    {
        return await _context.MatchTypes
            .OrderBy(mt => mt.Id)
            .Select(mt => new MatchTypeDto
            {
                Id = mt.Id,
                Name = mt.Name,
                PlayersPerSide = mt.PlayersPerSide
            })
            .ToListAsync();
    }
}
