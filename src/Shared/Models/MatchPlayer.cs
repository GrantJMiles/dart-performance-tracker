namespace DartPerformanceTracker.Shared.Models;

public class MatchPlayer
{
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
}
