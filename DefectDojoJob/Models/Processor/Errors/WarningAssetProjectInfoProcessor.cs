using DefectDojoJob.Services;

namespace DefectDojoJob.Models.Processor.Errors;

public class WarningAssetProjectInfoProcessor : Exception
{
    public string AssetIdentifier { get; set; }
    public EntityType EntityType { get; set; }
    public WarningAssetProjectInfoProcessor(string? message, string assetIdentifier, EntityType entityType) : base(message)
    {
        EntityType = entityType;
        AssetIdentifier = assetIdentifier;
    }

    //Needed for testing
    public WarningAssetProjectInfoProcessor()
    {
    }
}