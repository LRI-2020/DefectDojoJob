using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Processors;
using DefectDojoJob.Tests.AutoDataAttribute;
using FluentAssertions;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests;

public class EntitiesExtractorTests
{
    [Theory]
    [AutoMoqData]
    public void WhenTeamInPi_TeamAddedTpExtraction(AssetProjectInfo pi, EntitiesExtractor sut, string team)
    {
        pi.Team = team;
        var assetsPi = new List<AssetProjectInfo> { pi };

        var res = sut.ExtractEntities(assetsPi);

        res.Teams.Count.Should().Be(1);
        res.Teams.Should().Contain(team);
    }

    [Theory]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("  ")]
    [InlineAutoMoqData("")]
    public void WhenTeamNullOrEmpty_TeamDiscarded(string? team, AssetProjectInfo pi, EntitiesExtractor sut)
    {
        pi.Team = team;

        var assetsPi = new List<AssetProjectInfo> { pi };

        var res = sut.ExtractEntities(assetsPi);

        res.Teams.Count.Should().Be(0);
    }

    [Theory]
    [AutoMoqData]
    public void WhenUserInPi_UserAddedTpExtraction(AssetProjectInfo pi, EntitiesExtractor sut, string appOwner, string appOwnerBu, string functOwner)
    {
        pi.ApplicationOwner = appOwner;
        pi.ApplicationOwnerBackUp = appOwnerBu;
        pi.FunctionalOwner = functOwner;
        var assetsPi = new List<AssetProjectInfo> { pi };

        var res = sut.ExtractEntities(assetsPi);

        res.Users.Count.Should().Be(3);
        res.Users.Should().Contain(appOwner);
        res.Users.Should().Contain(appOwnerBu);
        res.Users.Should().Contain(functOwner);
    }

    [Theory]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("  ")]
    [InlineAutoMoqData("")]
    public void WhenUserNullOrEmpty_UserDiscarded(string? user, AssetProjectInfo pi, EntitiesExtractor sut)
    {
        pi.ApplicationOwner = user;
        pi.ApplicationOwnerBackUp = user;
        pi.FunctionalOwner = user;
        var assetsPi = new List<AssetProjectInfo> { pi };

        var res = sut.ExtractEntities(assetsPi);

        res.Users.Count.Should().Be(0);
    }

    [Theory]
    [AutoMoqData]
    public void WhenSeveralPi_AllExtracted(AssetProjectInfo pi1, AssetProjectInfo pi2, EntitiesExtractor sut)
    {

        var assetsPi = new List<AssetProjectInfo> { pi1,pi2 };

        var res = sut.ExtractEntities(assetsPi);

        res.Users.Count.Should().Be(6);
        res.Teams.Count.Should().Be(2);
    }
    
    
    [Theory]
    [AutoMoqData]
    public void WhenSeveralTimesSameUsername_AddedOnlyOnce(AssetProjectInfo pi1, AssetProjectInfo pi2, EntitiesExtractor sut, string username, string team)
    {

        pi1.ApplicationOwner = username;
        pi1.FunctionalOwner = username;
        pi2.FunctionalOwner = username;
        pi1.Team = team;
        pi2.Team = team;
        
        var assetsPi = new List<AssetProjectInfo> { pi1,pi2 };
        var res = sut.ExtractEntities(assetsPi);

        res.Users.ToList().FindAll(u => u == username).Count.Should().Be(1);
        res.Teams.ToList().FindAll(t => t == team).Count.Should().Be(1);
    }

}