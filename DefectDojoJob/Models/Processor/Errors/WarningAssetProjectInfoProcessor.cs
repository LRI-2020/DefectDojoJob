
namespace DefectDojoJob.Models.Processor.Errors;

public class WarningAssetProjectInfoProcessor : Exception
{
    public string AssetIdentifier { get; set; }
    public readonly EntityType EntityType;
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