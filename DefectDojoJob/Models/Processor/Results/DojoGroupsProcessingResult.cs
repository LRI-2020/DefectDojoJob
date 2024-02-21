using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public class DojoGroupsProcessingResult : IEntitiesProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
    public EntitiesType EntitiesType { get; } = EntitiesType.DojoGroup;
}