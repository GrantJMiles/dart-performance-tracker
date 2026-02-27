namespace DartPerformanceTracker.Shared.Models;

public class MatchType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PlayersPerSide { get; set; }
    public ICollection<SeasonMatchConfiguration> SeasonConfigurations { get; set; } = new List<SeasonMatchConfiguration>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
