using DartPerformanceTracker.Data;
using DartPerformanceTracker.Shared.Models;

namespace DartPerformanceTracker.Functions.Services;

/// <summary>
/// Populates an empty development database with a believable season so dashboards have something to show.
/// Only invoked for SQLite dev databases when SEED_DEV_DATA=true; does nothing if any team already exists.
/// </summary>
public static class DevDataSeeder
{
    private const int Singles = 1, Pairs = 2, Triples = 3;

    public static void Seed(AppDbContext db)
    {
        if (db.Teams.Any()) return;

        var team = new Team { Name = "The Oche Ops" };
        var players = new[] { "Dave Hargreaves", "Sarah Pike", "Mick O'Neill", "Priya Shah", "Tom Walker", "Jess Bailey" }
            .Select(n => new Player { Name = n, Team = team }).ToList();

        var season = new Season
        {
            Name = "2025/26",
            StartDate = new DateTime(2025, 9, 1),
            EndDate = new DateTime(2026, 4, 30),
            MatchConfigurations =
            {
                new SeasonMatchConfiguration { MatchTypeId = Singles, OrderIndex = 0 },
                new SeasonMatchConfiguration { MatchTypeId = Singles, OrderIndex = 1 },
                new SeasonMatchConfiguration { MatchTypeId = Pairs, OrderIndex = 2 },
                new SeasonMatchConfiguration { MatchTypeId = Triples, OrderIndex = 3 },
            }
        };

        db.Teams.Add(team);
        db.Players.AddRange(players);
        db.Seasons.Add(season);
        db.SaveChanges();

        var rng = new Random(42);
        var opponents = new[] { "Red Lion A", "The Crown", "Bullseye Bar", "Queens Head", "Railway Tavern" };

        for (var week = 0; week < 5; week++)
        {
            var night = new GameNight
            {
                SeasonId = season.Id,
                Date = new DateTime(2025, 9, 10).AddDays(week * 14),
                Opponent = opponents[week],
                IsHome = week % 2 == 0,
                IsComplete = true,
            };

            var order = 0;
            foreach (var config in season.MatchConfigurations.OrderBy(c => c.OrderIndex))
            {
                var perSide = config.MatchTypeId switch { Singles => 1, Pairs => 2, _ => 3 };
                var lineup = players.OrderBy(_ => rng.Next()).Take(perSide).ToList();
                var legsWon = rng.Next(0, 4);
                var legsLost = rng.Next(0, 4);
                if (legsWon == legsLost) legsWon++;

                var match = new Match
                {
                    MatchTypeId = config.MatchTypeId,
                    OrderIndex = order++,
                    LegsWon = legsWon,
                    LegsLost = legsLost,
                    Won = legsWon > legsLost,
                };

                foreach (var p in lineup)
                {
                    match.MatchPlayers.Add(new MatchPlayer { PlayerId = p.Id });
                    match.PlayerStats.Add(new PlayerMatchStats { PlayerId = p.Id, Tons = rng.Next(0, 5), Maximums = rng.Next(0, 12) == 0 ? 1 : 0 });
                }

                for (var leg = 0; leg < legsWon; leg++)
                {
                    match.Checkouts.Add(new LegCheckout { PlayerId = lineup[rng.Next(lineup.Count)].Id, Score = rng.Next(24, 171) });
                }

                night.Matches.Add(match);
            }

            night.ManOfTheMatchAwards.Add(new ManOfTheMatch { PlayerId = players[rng.Next(players.Count)].Id });
            db.GameNights.Add(night);
        }

        // One game night in progress so the match-day entry screen has something to resume.
        var inProgress = new GameNight
        {
            SeasonId = season.Id,
            Date = DateTime.Today,
            Opponent = "Dog & Duck",
            IsHome = true,
            IsComplete = false,
        };
        var slot = 0;
        foreach (var config in season.MatchConfigurations.OrderBy(c => c.OrderIndex))
        {
            inProgress.Matches.Add(new Match { MatchTypeId = config.MatchTypeId, OrderIndex = slot++ });
        }
        db.GameNights.Add(inProgress);

        db.SaveChanges();
    }
}
