using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Models.Adapters;

public class UsersAdaptersResults
{
    public UsersAdaptersResults(UsersProcessingResult usersProcessingResult, DojoGroupProcessingResult dojoGroupsProcessingResult)
    {
        UsersProcessingResult = usersProcessingResult;
        DojoGroupsProcessingResult = dojoGroupsProcessingResult;
    }

    public UsersProcessingResult UsersProcessingResult { get; set; }

    public DojoGroupProcessingResult DojoGroupsProcessingResult { get; set; }

}