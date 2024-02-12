using DefectDojoJob.Services;

namespace DefectDojoJob.Models.Processor.Errors;

public class WarningAssetProjectInfoProcessor : Exception
{
    private readonly EntityType entityType;
    public string AssetIdentifier { get; set; }
    public EntityType EntityType { get; set; }
    public WarningAssetProjectInfoProcessor(string? message, string assetIdentifier, EntityType entityType) : base(message)
    {
        this.entityType = entityType;
        AssetIdentifier = assetIdentifier;
    }
}