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
    public List<string> LastFiveForm { get; set; } = new();
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
    public int TotalTons { get; set; }
    public int TotalMaximums { get; set; }
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
    public TopTeammateDto? TopTeammate { get; set; }
    public List<PlayerMatchResultDto> RecentMatches { get; set; } = new();
}

public class TopTeammateDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int GamesPlayedTogether { get; set; }
    public int WinsTogether { get; set; }
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

public class DashboardInsightsDto
{
    public MvpDto? SinglesMvp { get; set; }
    public MvpDto? SeasonMvp { get; set; }
    public PairsMvpDto? PairsMvp { get; set; }
}

public class MvpDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public double WinPercentage { get; set; }
}

public class PairsMvpDto
{
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
}

public class TeamPlayerSeasonStatsDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int ManOfTheMatchCount { get; set; }
    public List<PlayerMatchTypeBreakdownDto> MatchTypeBreakdowns { get; set; } = new();
    public List<TopPairingDto> TopPairings { get; set; } = new();
}

public class PlayerMatchTypeBreakdownDto
{
    public string MatchTypeName { get; set; } = string.Empty;
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int LegsWon { get; set; }
    public int LegsLost { get; set; }
    public int Tons { get; set; }
    public int Maximums { get; set; }
}

public class TopPairingDto
{
    public string PartnerName { get; set; } = string.Empty;
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public double WinRatio { get; set; }
}
