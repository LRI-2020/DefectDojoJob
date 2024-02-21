using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Adapters;

public class UsersAdapter: IUsersAdapter
{
    private readonly IUsersProcessor usersProcessor;
    private readonly IGroupsProcessor groupsProcessor;
    private readonly IUsersExtractor usersExtractor;

    public UsersAdapter(IUsersProcessor usersProcessor, IGroupsProcessor groupsProcessor, IUsersExtractor usersExtractor)
    {
        this.usersProcessor = usersProcessor;
        this.groupsProcessor = groupsProcessor;
        this.usersExtractor = usersExtractor;
    }
    public async Task<UsersAdaptersResults> StartUsersAdapterAsync(List<AssetProject> assetProjectInfos)
    {
        var usernames = new HashSet<string>();
        var teamNames = new HashSet<string>();
        assetProjectInfos.ForEach(p =>
        {
            if (!string.IsNullOrEmpty(p.Team?.Trim())) teamNames.Add(p.Team);
            usernames.UnionWith(usersExtractor.ExtractValidUsernames(p));
        });
        var usersResult = await usersProcessor.ProcessUsersAsync(usernames.ToList()); 
        var groupsResult = await groupsProcessor.ProcessGroupsAsync(teamNames.ToList()); 
 
        return new UsersAdaptersResults(usersResult,groupsResult);
    }

    
}