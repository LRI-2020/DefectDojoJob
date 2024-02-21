namespace DefectDojoJob.Models.Processor.Results;

public interface IEntitiesProcessingResult : IProcessingResult
{
    List<AssetToDefectDojoMapper> Entities { get; set; }
}