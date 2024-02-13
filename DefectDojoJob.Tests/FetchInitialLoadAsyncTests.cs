using System.Net;
using AutoFixture.Xunit2;
using DefectDojoJob.Models.Processor.Interfaces;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DefectDojoJob.Tests;

public class FetchInitialLoadAsyncTests
{
    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData("2020")]
    public async Task WhenRefDateNotProvided_ExceptionThrown(string refDate, string url, HttpClient httpClient, IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        IConfiguration configuration = ConfigureInMemory(url, refDate);
        var sut = new InitialLoadService(httpClient, assetProjectInfoValidator, configuration);

        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.Errors.Count.Should().Be(1);
        res.Errors[0].error.Should().Contain("date");
    }

    [Theory]
    [InlineAutoMoqData("", "2020-04-02")]
    [InlineAutoMoqData("invalidUrl", "2020-04-02")]
    public async Task WhenInvalidUrl_ErrorAndNoEntityToProcess(string url, string refDate, HttpClient httpClient, IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        IConfiguration configuration = ConfigureInMemory(url, refDate);
        var sut = new InitialLoadService(httpClient, assetProjectInfoValidator, configuration);

        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.Errors.Count.Should().Be(1);
        res.Errors[0].error.Should().Contain("url");
    }

    [Theory, InlineAutoMoqData("./JsonInputFiles/AssetModelMissingRequired.json")]
    public async Task WhenAssetModelMissingRequiredProp_ErrorAndNoEntityToProcess(string jsonPath,IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        IConfiguration configuration = ConfigureInMemory("https://test.be", "2000-04-05");
        HttpClient httpClient = new HttpClient(GetFakeHandler(HttpStatusCode.Accepted,jsonPath));
        var sut = new InitialLoadService(httpClient, assetProjectInfoValidator, configuration);

        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.DiscardedProjects.Should().BeEmpty();
        res.Errors.Should().NotBeEmpty();
        res.Errors.ForEach(e =>
        {
            e.error.ToLower().Should().Contain("required");
        });
    }

    [Theory, InlineAutoMoqData("./JsonInputFiles/AssetModelIncorrectFormat.json")]
    public async Task WhenAssetModelInvalidFormat_ErrorAndNoEntityToProcess(string jsonPath,IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        IConfiguration configuration = ConfigureInMemory("https://test.be", "2000-04-05");
        HttpClient httpClient = new HttpClient(GetFakeHandler(HttpStatusCode.Accepted,jsonPath));
        var sut = new InitialLoadService(httpClient, assetProjectInfoValidator, configuration);

        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.DiscardedProjects.Should().BeEmpty();
        res.Errors.Should().NotBeEmpty();
        res.Errors.ForEach(e =>
        {
            e.error.ToLower().Should().Contain("convert");
        });
    }
    
 //Data is empty list
    //several JObject returned by private load and 
    // all valids
    // some or all invalids
    // cannot be parse to asset AssetProjectInfo
    // are AssetProjectInfo but invalid (Validate throw exception) --> 
    //some can be processed
    // some are discarded

    private IConfiguration ConfigureInMemory(string url, string refDate)
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AssetUrl", url },
            { "LastRunDate", refDate }
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    private IConfiguration ConfigureJsonFile(string jsonPath)
    {
        return new ConfigurationBuilder()
            .AddJsonFile(jsonPath)
            .Build();
    }

    private FakeHttpMessageHandler GetFakeHandler(HttpStatusCode statusCode, string jsonPath )
    {
        using StreamReader reader = new(jsonPath);
        var json = reader.ReadToEnd();
        return new FakeHttpMessageHandler(statusCode, json);
    }
}