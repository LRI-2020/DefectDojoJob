namespace DefectDojoJob.Services;

public class TeamEntityProcessingResult : IEntityProcessingResult
{
    public int DefectDojoId { get; set; }
    public string? AssetIdentifier { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool ProcessingSuccessful { get; set; }
    public EntityType EntityType { get; } = EntityType.Team;
}