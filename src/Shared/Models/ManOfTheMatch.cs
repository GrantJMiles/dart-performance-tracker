namespace DartPerformanceTracker.Shared.Models;

public class ManOfTheMatch
{
    public int GameNightId { get; set; }
    public GameNight GameNight { get; set; } = null!;
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
}
