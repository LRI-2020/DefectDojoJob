using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Adapters;

public class UsersAdapter: IUsersAdapter
{
    private readonly IUsersProcessor usersProcessor;
    private readonly IGroupsProcessor groupsProcessor;
    private readonly IUsersExtractor usersExtractor;
    private HashSet<string> Usernames { get; set; } = new();
    private HashSet<string> TeamNames { get; set; } = new();

    public UsersAdapter(IUsersProcessor usersProcessor, IGroupsProcessor groupsProcessor, IUsersExtractor usersExtractor)
    {
        this.usersProcessor = usersProcessor;
        this.groupsProcessor = groupsProcessor;
        this.usersExtractor = usersExtractor;
    }
    public async Task<UsersAdaptersResults> StartUsersAdapterAsync(List<AssetProject> assetProjectInfos)
    {
        assetProjectInfos.ForEach(p =>
        {
            if (!string.IsNullOrEmpty(p.Team?.Trim())) TeamNames.Add(p.Team);
            Usernames.UnionWith(usersExtractor.ExtractValidUsernames(p));
        });
        var usersResult = await usersProcessor.ProcessUsersAsync(Usernames.ToList()); 
        var groupsResult = await groupsProcessor.ProcessGroupsAsync(TeamNames.ToList()); 
 
        return new UsersAdaptersResults(usersResult,groupsResult);
    }

    
}