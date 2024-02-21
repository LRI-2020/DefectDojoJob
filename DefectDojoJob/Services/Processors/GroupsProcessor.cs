using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class GroupsProcessor: IGroupsProcessor
{
    public Task<DojoGroupProcessingResult> ProcessGroupsAsync(List<string> toList)
    {
        throw new NotImplementedException();
    }
}