using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Models.Adapters;

public class UsersAdaptersResults
{
    public UsersAdaptersResults(UsersProcessingResult usersProcessingResult, DojoGroupsProcessingResult dojoGroupsProcessingResult)
    {
        UsersProcessingResult = usersProcessingResult;
        DojoGroupsProcessingResult = dojoGroupsProcessingResult;
    }

    public UsersProcessingResult UsersProcessingResult { get; set; }

    public DojoGroupsProcessingResult DojoGroupsProcessingResult { get; set; }

}