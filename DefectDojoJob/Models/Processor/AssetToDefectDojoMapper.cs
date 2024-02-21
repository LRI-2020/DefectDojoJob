namespace DefectDojoJob.Models.Processor;

public class AssetToDefectDojoMapper
{

    public AssetToDefectDojoMapper(string assetIdentifier, int defectDojoId, EntitiesType entitiesType)
    {
        EntitiesType = entitiesType;
        AssetIdentifier = assetIdentifier;
        DefectDojoId = defectDojoId;
    }

    public int DefectDojoId { get; set; }
    public readonly EntitiesType EntitiesType;
   public string AssetIdentifier { get; set; }
}

public class AssetToProductMapper : AssetToDefectDojoMapper
{
    public AssetToProductMapper(string assetIdentifier, int defectDojoId) : base(assetIdentifier, defectDojoId, EntitiesType.Product)
    {
    }
}

public class AssetToMetadataMapper : AssetToDefectDojoMapper
{
    public AssetToMetadataMapper(string assetIdentifier, int defectDojoId) : 
        base(assetIdentifier, defectDojoId, EntitiesType.Metadata)
    {
    }
}