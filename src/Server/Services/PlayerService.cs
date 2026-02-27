using DartPerformanceTracker.Server.Data;
using DartPerformanceTracker.Shared.DTOs;
using DartPerformanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Server.Services;

public class PlayerService : IPlayerService
{
    private readonly AppDbContext _context;

    public PlayerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlayerDto>> GetAllAsync()
    {
        return await _context.Players
            .Include(p => p.Team)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.Name,
                TeamId = p.TeamId,
                TeamName = p.Team.Name,
                IsActive = p.IsActive
            })
            .ToListAsync();
    }

    public async Task<PlayerDto?> GetByIdAsync(int id)
    {
        var player = await _context.Players.Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == id);
        if (player == null) return null;
        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            TeamId = player.TeamId,
            TeamName = player.Team.Name,
            IsActive = player.IsActive
        };
    }

    public async Task<PlayerDto> CreateAsync(CreatePlayerDto dto)
    {
        var player = new Player { Name = dto.Name, TeamId = dto.TeamId, IsActive = dto.IsActive };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        var team = await _context.Teams.FindAsync(dto.TeamId);
        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            TeamId = player.TeamId,
            TeamName = team?.Name ?? string.Empty,
            IsActive = player.IsActive
        };
    }
}
