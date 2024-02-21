
namespace DefectDojoJob.Models.Processor.Errors;

public class WarningAssetProjectProcessor : Exception
{
    public string AssetIdentifier { get; set; }
    public readonly EntitiesType EntitiesType; 
    public WarningAssetProjectProcessor(string? message, string assetIdentifier, EntitiesType? entitiesType=null) : base(message)
    {
        EntitiesType = entitiesType ?? EntitiesType.Unknown;
        AssetIdentifier = assetIdentifier;
    }

    //Needed for testing
    public WarningAssetProjectProcessor()
    {
    }
}