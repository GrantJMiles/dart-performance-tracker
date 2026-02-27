namespace DartPerformanceTracker.Shared.DTOs;

public class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreatePlayerDto
{
    public string Name { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public bool IsActive { get; set; } = true;
}
