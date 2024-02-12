using DefectDojoJob.Models;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Services;

public class UsersProcessingResult: IEntitiesProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } = new();
    public bool ProcessingSuccessful { get; set; }
    public EntityType EntityType { get; } = EntityType.User;
}