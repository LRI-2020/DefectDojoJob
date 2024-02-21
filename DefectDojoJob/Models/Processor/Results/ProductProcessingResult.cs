using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public class ProductProcessingResult:IEntityProcessingResult
{
    public AssetToDefectDojoMapper? Entity { get; set; }
    public List<AssetToMetadataMapper> MetadataMappers { get; set; } = new();
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
    public EntitiesType EntitiesType { get; } = EntitiesType.Product;
}