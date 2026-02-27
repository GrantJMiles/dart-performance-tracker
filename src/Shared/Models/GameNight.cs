namespace DartPerformanceTracker.Shared.Models;

public class GameNight
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public bool IsHome { get; set; }
    public ICollection<Match> Matches { get; set; } = new List<Match>();
    public ICollection<ManOfTheMatch> ManOfTheMatchAwards { get; set; } = new List<ManOfTheMatch>();
}
