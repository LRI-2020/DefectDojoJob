namespace DefectDojoJob.Services;

public class UsersProcessingResult: IEntitiesProcessingResult
{
    public List<(string AssetIdentifier, int DefectDojoId)> Entities { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool ProcessingSuccessful { get; set; }
    public EntityType EntityType { get; } = EntityType.User;
}