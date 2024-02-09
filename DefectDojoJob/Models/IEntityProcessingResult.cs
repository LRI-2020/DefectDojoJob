namespace DefectDojoJob.Services;

public interface IEntityProcessingResult
{
    public int DefectDojoId { get; set; }
    public string? AssetIdentifier { get; set; }
    public List<string> Errors { get; set; }
    public List<string> Warnings { get; set; }
    public bool ProcessingSuccessful { get; set; }
    public EntityType EntityType { get; }
}