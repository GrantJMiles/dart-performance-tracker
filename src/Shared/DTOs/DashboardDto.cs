namespace DartPerformanceTracker.Shared.DTOs;

public class TeamDashboardDto
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public int TotalGameNights { get; set; }
    public int GameNightsWon { get; set; }
    public int GameNightsLost { get; set; }
    public int GameNightsDrawn { get; set; }
    public int TotalMatchesPlayed { get; set; }
    public int TotalMatchesWon { get; set; }
    public int TotalMatchesLost { get; set; }
    public List<GameNightSummaryDto> GameNightSummaries { get; set; } = new();
}

public class GameNightSummaryDto
{
    public int GameNightId { get; set; }
    public DateTime Date { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public bool IsHome { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public bool Won { get; set; }
}

public class PlayerDashboardDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public int TotalTons { get; set; }
    public int TotalMaximums { get; set; }
    public int ManOfTheMatchCount { get; set; }
    public List<PlayerMatchResultDto> RecentMatches { get; set; } = new();
}

public class PlayerMatchResultDto
{
    public int MatchId { get; set; }
    public DateTime Date { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public string MatchTypeName { get; set; } = string.Empty;
    public bool Won { get; set; }
    public int Tons { get; set; }
    public int Maximums { get; set; }
}
