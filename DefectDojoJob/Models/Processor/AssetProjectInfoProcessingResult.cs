using DefectDojoJob.Models;

namespace DefectDojoJob.Services;

public class AssetProjectInfoEntityProcessingResult : IEntitiesProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } = new();
    public EntityType EntityType { get; } = new();
}