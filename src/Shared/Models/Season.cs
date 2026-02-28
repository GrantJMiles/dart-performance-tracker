namespace DartPerformanceTracker.Shared.Models;

public class Season
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaximumScore { get; set; } = 180;
    public ICollection<SeasonMatchConfiguration> MatchConfigurations { get; set; } = new List<SeasonMatchConfiguration>();
    public ICollection<GameNight> GameNights { get; set; } = new List<GameNight>();
}
