using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public class ProductProcessingResult:IEntitiesProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } = new();
    public EntityType EntityType { get; } = EntityType.Product;
}