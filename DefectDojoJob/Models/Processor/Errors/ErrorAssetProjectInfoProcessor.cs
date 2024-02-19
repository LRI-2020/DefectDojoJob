
namespace DefectDojoJob.Models.Processor.Errors;

public class ErrorAssetProjectInfoProcessor:Exception
{
    public readonly EntityType EntityType;
    public string AssetIdentifier { get;}
    public ErrorAssetProjectInfoProcessor(string? message, string assetIdentifier, EntityType entityType) : base(message)
    {
        EntityType = entityType;
        AssetIdentifier = assetIdentifier;
    }
}