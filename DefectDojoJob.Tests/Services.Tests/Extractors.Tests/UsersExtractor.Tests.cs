using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Adapters;
using DefectDojoJob.Services.Extractors;
using DefectDojoJob.Tests.AutoDataAttribute;
using FluentAssertions;

namespace DefectDojoJob.Tests.Services.Tests.Extractors.Tests;

public class UsersExtractorTests
{
    [Theory]
    [AutoMoqData]
    public void WhenUserInPi_UserAddedTpExtraction(AssetProject pi, UsersExtractor sut, string appOwner, string appOwnerBu, string functOwner)
    {
        pi.ApplicationOwner = appOwner;
        pi.ApplicationOwnerBackUp = appOwnerBu;
        pi.FunctionalOwner = functOwner;

        var res = sut.ExtractValidUsernames(pi);

        res.Count.Should().Be(3);
        res.Should().Contain(appOwner);
        res.Should().Contain(appOwnerBu);
        res.Should().Contain(functOwner);
    }

    [Theory]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("  ")]
    [InlineAutoMoqData("")]
    public void WhenUserNullOrEmpty_UserDiscarded(string? user, AssetProject pi, UsersExtractor sut)
    {
        pi.ApplicationOwner = user;
        pi.ApplicationOwnerBackUp = user;
        pi.FunctionalOwner = user;

        var res = sut.ExtractValidUsernames(pi);

        res.Count.Should().Be(0);
    }


    
    
    [Theory]
    [AutoMoqData]
    public void WhenSeveralTimesSameUsername_AddedOnlyOnce(AssetProject pi, UsersExtractor sut, string username)
    {

        pi.ApplicationOwner = username;
        pi.FunctionalOwner = username;
        pi.ApplicationOwnerBackUp = username;
        
        var res = sut.ExtractValidUsernames(pi);

        res.Count.Should().Be(1);
    }

}