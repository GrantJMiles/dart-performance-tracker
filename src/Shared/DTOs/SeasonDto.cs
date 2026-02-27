namespace DartPerformanceTracker.Shared.DTOs;

public class SeasonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<SeasonMatchConfigurationDto> MatchConfigurations { get; set; } = new();
}

public class CreateSeasonDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CreateSeasonMatchConfigurationDto> MatchConfigurations { get; set; } = new();
}

public class SeasonMatchConfigurationDto
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int MatchTypeId { get; set; }
    public string MatchTypeName { get; set; } = string.Empty;
    public int PlayersPerSide { get; set; }
    public int NumberOfMatches { get; set; }
    public int OrderIndex { get; set; }
}

public class CreateSeasonMatchConfigurationDto
{
    public int MatchTypeId { get; set; }
    public int NumberOfMatches { get; set; }
    public int OrderIndex { get; set; }
}
