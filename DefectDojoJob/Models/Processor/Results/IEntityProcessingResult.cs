using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public interface IEntityProcessingResult
{
    public AssetToDefectDojoMapper? Entity { get; set; }
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; }
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } 
    public EntityType EntityType { get; }
}