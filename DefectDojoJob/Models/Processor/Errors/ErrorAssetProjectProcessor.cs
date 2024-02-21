
namespace DefectDojoJob.Models.Processor.Errors;

public class ErrorAssetProjectProcessor:Exception
{
    public readonly EntitiesType EntitiesType;
    public string AssetIdentifier { get;}
    public ErrorAssetProjectProcessor(string? message, string assetIdentifier, EntitiesType? entitiesType=null) : base(message)
    {
        EntitiesType = entitiesType ?? EntitiesType.Unknown;
        AssetIdentifier = assetIdentifier;
    }
}