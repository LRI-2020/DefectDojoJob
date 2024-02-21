namespace DefectDojoJob.Tests.Services.Tests.Adapters.Tests;

public class UsersAdapterTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenTeamNotNull_TeamProcessed([Frozen]Mock<IGroupsProcessor> groupsProcessorMock, 
        AssetProject pi, UsersAdapter sut, string team)
    {
 
        pi.Team = team;
        var assetsPi = new List<AssetProject> { pi };

        await sut.StartUsersAdapterAsync(assetsPi);
        
        groupsProcessorMock.Verify(m=>m.ProcessGroupsAsync(It.Is<List<string>>(ls=>ls.Contains(team))));

    }
    
    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData("  ")]
    [InlineAutoMoqData(null)]
    public async Task WhenTeamNull_TeamNotProcessed(string?team,[Frozen]Mock<IGroupsProcessor> groupsProcessorMock, 
        AssetProject pi, UsersAdapter sut)
    {
 
        pi.Team = team;
        var assetsPi = new List<AssetProject> { pi };

        await sut.StartUsersAdapterAsync(assetsPi);
        
        groupsProcessorMock.Verify(m=>m.ProcessGroupsAsync(It.Is<List<string>>(ls=>ls.Count==0)));

    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenSeveralDifferentTeam_AllProcessed([Frozen]Mock<IGroupsProcessor> groupsProcessorMock, 
        AssetProject pi, AssetProject pi2, UsersAdapter sut, string team, string team2)
    {
 
        pi.Team = team;
        pi2.Team = team2;
        var assetsPi = new List<AssetProject> { pi,pi2 };

        await sut.StartUsersAdapterAsync(assetsPi);
        
        groupsProcessorMock.Verify(m=>m.ProcessGroupsAsync(It.Is<List<string>>(ls=>ls.Count==2)));

    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenSeveralSameTeam_ProcessedOnce([Frozen]Mock<IGroupsProcessor> groupsProcessorMock, 
        AssetProject pi, AssetProject pi2, UsersAdapter sut, string team)
    {
 
        pi.Team = team;
        pi2.Team = team;
        var assetsPi = new List<AssetProject> { pi,pi2 };

        await sut.StartUsersAdapterAsync(assetsPi);
        
        groupsProcessorMock.Verify(m=>m.ProcessGroupsAsync(It.Is<List<string>>(ls=>ls.Count==1)));

    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenSeveralPi_AllValidUsersProcessed([Frozen]Mock<IUsersProcessor> usersProcessorMock, 
        [Frozen]Mock<IUsersExtractor> usersExtractorMock,
        AssetProject pi, AssetProject pi2, UsersAdapter sut)
    {
        usersExtractorMock.Setup(m=>m.ExtractValidUsernames(pi)).Returns(new HashSet<string>{"test"});
        usersExtractorMock.Setup(m=>m.ExtractValidUsernames(pi2)).Returns(new HashSet<string>{"test2"});
 
        var assetsPi = new List<AssetProject> { pi,pi2 };

        await sut.StartUsersAdapterAsync(assetsPi);
        
        usersProcessorMock.Verify(
            m=>m.ProcessUsersAsync(It.Is<List<string>>(ls=>ls.Count==2)));

    }

}