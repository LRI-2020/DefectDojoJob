using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Services.Interfaces;

public interface IGroupsProcessor
{
    Task<DojoGroupProcessingResult> ProcessGroupsAsync(List<string> toList);
}