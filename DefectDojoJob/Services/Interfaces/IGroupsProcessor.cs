using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Services.Interfaces;

public interface IGroupsProcessor
{
    Task<DojoGroupsProcessingResult> ProcessGroupsAsync(List<string> teamNames);
}