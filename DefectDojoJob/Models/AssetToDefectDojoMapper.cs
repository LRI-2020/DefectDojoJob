namespace DefectDojoJob.Models;

public class AssetToDefectDojoMapper
{
    public AssetToDefectDojoMapper(string assetIdentifier, int defectDojoId)
    {
        AssetIdentifier = assetIdentifier;
        DefectDojoId = defectDojoId;
    }

    public int DefectDojoId { get; set; }
    public string AssetIdentifier { get; set; }
}