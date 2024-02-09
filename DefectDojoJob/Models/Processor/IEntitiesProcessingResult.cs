namespace DefectDojoJob.Services;

public interface IEntitiesProcessingResult
{
    public List<(string AssetIdentifier, int DefectDojoId)> Entities { get; set; }
    public List<string> Errors { get; set; }
    public List<string> Warnings { get; set; } 
    public EntityType EntityType { get; }
}