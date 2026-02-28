namespace DartPerformanceTracker.Shared.Models;

public class Match
{
    public int Id { get; set; }
    public int GameNightId { get; set; }
    public GameNight GameNight { get; set; } = null!;
    public int MatchTypeId { get; set; }
    public MatchType MatchType { get; set; } = null!;
    public int LegsWon { get; set; }
    public int LegsLost { get; set; }
    public bool Won { get; set; }
    public int OrderIndex { get; set; }
    public ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
    public ICollection<PlayerMatchStats> PlayerStats { get; set; } = new List<PlayerMatchStats>();
}
