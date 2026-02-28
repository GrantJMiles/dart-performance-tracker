using DartPerformanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DartPerformanceTracker.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Shared.Models.MatchType> MatchTypes => Set<Shared.Models.MatchType>();
    public DbSet<SeasonMatchConfiguration> SeasonMatchConfigurations => Set<SeasonMatchConfiguration>();
    public DbSet<GameNight> GameNights => Set<GameNight>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchPlayer> MatchPlayers => Set<MatchPlayer>();
    public DbSet<PlayerMatchStats> PlayerMatchStats => Set<PlayerMatchStats>();
    public DbSet<ManOfTheMatch> ManOfTheMatches => Set<ManOfTheMatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MatchPlayer>()
            .HasKey(mp => new { mp.MatchId, mp.PlayerId });

        modelBuilder.Entity<ManOfTheMatch>()
            .HasKey(m => new { m.GameNightId, m.PlayerId });

        modelBuilder.Entity<Match>()
            .HasIndex(m => m.GameNightId);

        modelBuilder.Entity<MatchPlayer>()
            .HasIndex(mp => mp.PlayerId);

        modelBuilder.Entity<PlayerMatchStats>()
            .HasIndex(ps => ps.PlayerId);

        modelBuilder.Entity<GameNight>()
            .HasIndex(gn => gn.SeasonId);

        modelBuilder.Entity<Shared.Models.MatchType>().HasData(
            new Shared.Models.MatchType { Id = 1, Name = "Singles", PlayersPerSide = 1 },
            new Shared.Models.MatchType { Id = 2, Name = "Pairs", PlayersPerSide = 2 },
            new Shared.Models.MatchType { Id = 3, Name = "Triples", PlayersPerSide = 3 },
            new Shared.Models.MatchType { Id = 4, Name = "Fours", PlayersPerSide = 4 },
            new Shared.Models.MatchType { Id = 5, Name = "Sixes", PlayersPerSide = 6 },
            new Shared.Models.MatchType { Id = 6, Name = "Blind Pairs", PlayersPerSide = 2 }
        );
    }
}
