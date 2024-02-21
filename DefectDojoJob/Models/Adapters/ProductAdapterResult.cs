using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Models.Adapters;

public class ProductAdapterResult
{
    public ProductProcessingResult ProductResult { get; set; } = new();
    public MetadataProcessingResult MetadataResults { get; set; } = new();
    public EngagementsProcessingResult EngagementsResults { get; set; } = new();
    public EndpointsProcessingResults EndpointsProcessingResults { get; set; } = new();
    public HashSet<Endpoint> Endpoints { get; set; } = new();
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
}

public class EndpointsProcessingResults:IEntitiesProcessingResult
{
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
    public EntitiesType EntitiesType { get; } = EntitiesType.Endpoint;
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
}

public class EngagementsProcessingResult:IEntitiesProcessingResult
{
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
    public EntitiesType EntitiesType { get; } = EntitiesType.Engagement;
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
}

public class MetadataProcessingResult:IEntitiesProcessingResult
{
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
    public EntitiesType EntitiesType { get; } = EntitiesType.Metadata;
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
}