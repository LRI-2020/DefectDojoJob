using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public class ProductProcessingResult:IEntityProcessingResult
{
    public AssetToDefectDojoMapper? Entity { get; set; }
    public List<AssetToMetadataMapper> MetadataMappers { get; set; } = new();
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } = new();
    public EntityType EntityType { get; } = EntityType.Product;
}