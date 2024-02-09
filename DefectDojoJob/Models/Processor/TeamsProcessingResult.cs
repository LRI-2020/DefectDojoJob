using DefectDojoJob.Models;

namespace DefectDojoJob.Services;

public class TeamsProcessingResult : IEntitiesProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } = new();
    public EntityType EntityType { get; } = EntityType.Team;
}