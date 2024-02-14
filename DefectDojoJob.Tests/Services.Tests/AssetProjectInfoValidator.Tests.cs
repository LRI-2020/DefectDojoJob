using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using FluentAssertions;

namespace DefectDojoJob.Tests.Services.Tests;

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

    [Theory]
    [InlineAutoMoqData("2020,01,02","2020-01-03","2020-01-03")]
    [InlineAutoMoqData("2020,01,02","2020-01-03","2020-01-01")]
    [InlineAutoMoqData("2020,01,02","2020-01-01","2020-01-03")]
    public void WhenAtLeastOneDateGreaterThanRefDate_ShouldBeProcessed(string lastRunDate, string created, string updated,AssetProjectInfo pi, AssetProjectInfoValidator sut)
    {
        var refDate = new DateTimeOffset(DateTime.Parse(lastRunDate), new TimeSpan(0));
        pi.Created = new DateTimeOffset(DateTime.Parse(created), new TimeSpan(0));
        pi.Updated = new DateTimeOffset(DateTime.Parse(updated),new TimeSpan(0));

        var res = sut.ShouldBeProcessed(refDate, pi);

        res.Should().BeTrue();
    }
    [Theory]
    [InlineAutoMoqData("2020,01,02","2020-01-02","2020-01-02")]
    [InlineAutoMoqData("2020,01,02","2020-01-01","2020-01-01")]
    public void WhenDatesLessThanOrEqualToRefDate_ShouldNotBeProcessed(string lastRunDate, string created, string updated,AssetProjectInfo pi, AssetProjectInfoValidator sut)
    {
        var refDate = new DateTimeOffset(DateTime.Parse(lastRunDate), new TimeSpan(0));
        pi.Created = new DateTimeOffset(DateTime.Parse(created), new TimeSpan(0));
        pi.Updated = new DateTimeOffset(DateTime.Parse(updated),new TimeSpan(0));

        var res = sut.ShouldBeProcessed(refDate, pi);

        res.Should().BeFalse();
    }
}