namespace DartPerformanceTracker.Shared.DTOs;

public class GameNightDto
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public DateTime Date { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public bool IsHome { get; set; }
    public List<MatchDto> Matches { get; set; } = new();
    public List<int> ManOfTheMatchPlayerIds { get; set; } = new();
}

public class CreateGameNightDto
{
    public int SeasonId { get; set; }
    public DateTime Date { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public bool IsHome { get; set; }
    public List<CreateMatchDto> Matches { get; set; } = new();
    public List<int> ManOfTheMatchPlayerIds { get; set; } = new();
}

public class MatchDto
{
    public int Id { get; set; }
    public int GameNightId { get; set; }
    public int MatchTypeId { get; set; }
    public string MatchTypeName { get; set; } = string.Empty;
    public int LegsWon { get; set; }
    public int LegsLost { get; set; }
    public bool Won { get; set; }
    public List<int> PlayerIds { get; set; } = new();
    public List<PlayerMatchStatsDto> PlayerStats { get; set; } = new();
}

public class CreateMatchDto
{
    public int MatchTypeId { get; set; }
    public int LegsWon { get; set; }
    public int LegsLost { get; set; }
    public bool Won { get; set; }
    public List<int> PlayerIds { get; set; } = new();
    public List<CreatePlayerMatchStatsDto> PlayerStats { get; set; } = new();
}

public class PlayerMatchStatsDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Tons { get; set; }
    public int Maximums { get; set; }
}

public class CreatePlayerMatchStatsDto
{
    public int PlayerId { get; set; }
    public int Tons { get; set; }
    public int Maximums { get; set; }
}
