namespace DartPerformanceTracker.Shared.Models;

public class SeasonMatchConfiguration
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
    public int MatchTypeId { get; set; }
    public MatchType MatchType { get; set; } = null!;
    public int NumberOfMatches { get; set; }
    public int OrderIndex { get; set; }
}
