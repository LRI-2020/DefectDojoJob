namespace DefectDojoJob.Models.Processor;

public class AssetToDefectDojoMapper
{

    public AssetToDefectDojoMapper(string assetIdentifier, int defectDojoId, EntityType entityType)
    {
        EntityType = entityType;
        AssetIdentifier = assetIdentifier;
        DefectDojoId = defectDojoId;
    }

    public int DefectDojoId { get; set; }
    public readonly EntityType EntityType;
   public string AssetIdentifier { get; set; }
}

public class AssetToProductMapper : AssetToDefectDojoMapper
{
    public AssetToProductMapper(string assetIdentifier, int defectDojoId) : base(assetIdentifier, defectDojoId, EntityType.Product)
    {
    }
}

public class AssetToMetadataMapper : AssetToDefectDojoMapper
{
    public AssetToMetadataMapper(string assetIdentifier, int defectDojoId) : 
        base(assetIdentifier, defectDojoId, EntityType.Metadata)
    {
    }
}