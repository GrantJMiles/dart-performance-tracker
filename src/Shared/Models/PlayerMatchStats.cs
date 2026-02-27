namespace DartPerformanceTracker.Shared.Models;

public class PlayerMatchStats
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public int Tons { get; set; }
    public int Maximums { get; set; }
}
