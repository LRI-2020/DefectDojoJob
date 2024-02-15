using DefectDojoJob.Models;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Services;

public class AssetProjectInfoEntityProcessingResult : IEntitiesProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } = new();
    public EntityType EntityType { get; } = new();
}