using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public class ProductProcessingResult:IEntityProcessingResult
{
    public AssetToDefectDojoMapper? Entity { get; set; }
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
    public EntitiesType EntitiesType { get; } = EntitiesType.Product;
    public ProductAdapterAction ProductAdapterAction { get; set; } = ProductAdapterAction.None;
}