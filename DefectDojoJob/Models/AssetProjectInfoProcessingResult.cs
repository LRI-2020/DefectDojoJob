namespace DefectDojoJob.Services;

public class AssetProjectInfoEntityProcessingResult : IEntityProcessingResult
{
    public string? AssetIdentifier { get; set; }
    public TeamEntityProcessingResult TeamEntityProcessingResult { get; set; }
    public UserEntityProcessingResult ApplicationOwnerEntityProcessingResult { get; set; }
    public UserEntityProcessingResult ApplicationOwnerBuEntityProcessingResult { get; set; }
    public UserEntityProcessingResult FunctionalOwnerEntityProcessingResult { get; set; }
    public AssetProjectInfoProcessingAction Action { get; set; }
    public int DefectDojoId { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool ProcessingSuccessful { get; set; }
    public EntityType EntityType { get; } = EntityType.Product;
}