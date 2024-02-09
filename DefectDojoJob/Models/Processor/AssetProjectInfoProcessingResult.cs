namespace DefectDojoJob.Services;

public class AssetProjectInfoEntityProcessingResult : IEntitiesProcessingResult
{
    public List<(string AssetIdentifier, int DefectDojoId)> Entities { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public EntityType EntityType { get; } = new();
}