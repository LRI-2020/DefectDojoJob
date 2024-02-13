using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using FluentAssertions;

namespace DefectDojoJob.Tests;

public class AssetProjectInfoValidatorTests
{
    [Theory]
    [AutoMoqData]
    public void WhenProjectIdLowerThanZero_Exception(AssetProjectInfo pi)
    {
        pi.Id = -1;
        var sut = new AssetProjectInfoValidator();

        var act = () => sut.Validate(pi);
        act.Should().Throw<Exception>().Where(e => e.Message.ToLower().Contains("invalid") &&
                                                   e.Message.ToLower().Contains("id"));
        ;
    }

    [Theory]
    [InlineAutoMoqData("  ")]
    [InlineAutoMoqData("")]
    public void WhenNameIsEmpty_Exception(string name, AssetProjectInfo pi)
    {
        pi.Name = name;
        var sut = new AssetProjectInfoValidator();

        var act = () => sut.Validate(pi);
        act.Should().Throw<Exception>().Where(e => e.Message.ToLower().Contains("invalid") &&
                                                   e.Message.ToLower().Contains("name"));
        ;
    }

    [Theory]
    [AutoMoqData]
    public void WhenNoDescription_Exception(AssetProjectInfo pi)
    {
        pi.ShortDescription = null;
        pi.DetailedDescription = null;
        var sut = new AssetProjectInfoValidator();

        var act = () => sut.Validate(pi);
        act.Should().Throw<Exception>().Where(e => e.Message.ToLower().Contains("invalid") &&
                                                   e.Message.ToLower().Contains("description"));
        ;
    }

    [Theory]
    [InlineAutoMoqData("short","detailed",0)]
    [InlineAutoMoqData("short",null,1)]
    [InlineAutoMoqData(null,"detailed",1)]
    public void WhenValid_NoException(string? shortDesc, string? detailedDesc, int id, AssetProjectInfo pi)
    {
        pi.Name = "test";
        pi.Id = id;
        pi.DetailedDescription = detailedDesc;
        pi.ShortDescription = shortDesc;
        var sut = new AssetProjectInfoValidator();

        var act = () => sut.Validate(pi);
        act.Should().NotThrow<Exception>();
    }
    //ShouldBeProcessedMethod:
    //Created Date and Updated Date older or equal to refDate
    // CreatedDate or Updated Date null (cannot be?)
    // CReate Date or Updated Date newer than RefDate
}