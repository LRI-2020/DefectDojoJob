using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public interface IEntityProcessingResult : IProcessingResult
{
    public AssetToDefectDojoMapper? Entity { get; set; }

}

public interface IProcessingResult
{
    public List<ErrorAssetProjectProcessor> Errors { get; set; }
    public List<WarningAssetProjectProcessor> Warnings { get; set; } 
    public EntitiesType EntitiesType { get; }
}