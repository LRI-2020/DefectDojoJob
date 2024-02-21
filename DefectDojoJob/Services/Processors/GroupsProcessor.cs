using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class GroupsProcessor : IGroupsProcessor
{
    public async Task<DojoGroupsProcessingResult> ProcessGroupsAsync(List<string> teamNames)
    {
        // TODO Implement
        var res = await Task.Run(() => new DojoGroupsProcessingResult());
        return res;
    }
}