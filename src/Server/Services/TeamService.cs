using DartPerformanceTracker.Server.Data;
using DartPerformanceTracker.Shared.DTOs;
using DartPerformanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Server.Services;

public class TeamService : ITeamService
{
    private readonly AppDbContext _context;

    public TeamService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeamDto>> GetAllAsync()
    {
        return await _context.Teams
            .Select(t => new TeamDto { Id = t.Id, Name = t.Name })
            .ToListAsync();
    }

    public async Task<TeamDto?> GetByIdAsync(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team == null) return null;
        return new TeamDto { Id = team.Id, Name = team.Name };
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto dto)
    {
        var team = new Team { Name = dto.Name };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        return new TeamDto { Id = team.Id, Name = team.Name };
    }
}
