namespace DartPerformanceTracker.Shared.Models;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
    public ICollection<PlayerMatchStats> Stats { get; set; } = new List<PlayerMatchStats>();
    public ICollection<ManOfTheMatch> ManOfTheMatchAwards { get; set; } = new List<ManOfTheMatch>();
}
