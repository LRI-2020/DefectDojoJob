
namespace DefectDojoJob.Models.Processor.Errors;

public class WarningAssetProjectProcessor : Exception
{
    public string AssetIdentifier { get; set; }
    public readonly EntitiesType EntitiesType;
    public WarningAssetProjectProcessor(string? message, string assetIdentifier, EntitiesType entitiesType) : base(message)
    {
        EntitiesType = entitiesType;
        AssetIdentifier = assetIdentifier;
    }

    //Needed for testing
    public WarningAssetProjectProcessor()
    {
    }
}