using System.Net;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Interfaces;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DefectDojoJob.Tests.Services.Tests;

public class FetchInitialLoadAsyncTests
{
    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData("2020")]
    public async Task WhenRefDateNotProvided_ExceptionThrown(string refDate, string url, HttpClient httpClient, IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        IConfiguration configuration = TestHelper.ConfigureInMemory(url, refDate);
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
        IConfiguration configuration = TestHelper.ConfigureInMemory(url, refDate);
        var sut = new InitialLoadService(httpClient, assetProjectInfoValidator, configuration);

        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.Errors.Count.Should().Be(1);
        res.Errors[0].error.Should().Contain("url");
    }

    [Theory, InlineAutoMoqData("./TestInputs/AssetModelMissingRequired.json")]
    public async Task WhenAssetModelMissingRequiredProp_ErrorAndNoEntityToProcess(string jsonPath,IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        var sut = SutWithFakeHandler(jsonPath, assetProjectInfoValidator);
        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.DiscardedProjects.Should().BeEmpty();
        res.Errors.Should().NotBeEmpty();
        res.Errors.ForEach(e =>
        {
            e.error.ToLower().Should().Contain("required");
        });
    }

    [Theory, InlineAutoMoqData("./TestInputs/AssetModelIncorrectFormat.json")]
    public async Task WhenAssetModelInvalidFormat_ErrorAndNoEntityToProcess(string jsonPath,IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        var sut = SutWithFakeHandler(jsonPath, assetProjectInfoValidator);
        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.DiscardedProjects.Should().BeEmpty();
        res.Errors.Should().NotBeEmpty();
        res.Errors.ForEach(e =>
        {
            e.error.ToLower().Should().Contain("convert");
        });
    }
    
    [Theory, InlineAutoMoqData("./TestInputs/ValidAssetProjectInformation.json")]
    public async Task WhenAssetCannotBeValidated_ErrorAndEntityNotProcessed(string jsonPath,Mock<IAssetProjectInfoValidator> assetProjectInfoValidatorMock)
    {
        assetProjectInfoValidatorMock.Setup(m => m.Validate(It.IsAny<AssetProjectInfo>()))
            .Throws(new Exception());
        var sut = SutWithFakeHandler(jsonPath, assetProjectInfoValidatorMock.Object);
        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Should().BeEmpty();
        res.DiscardedProjects.Should().BeEmpty();
        res.Errors.Count.Should().Be(1);
        
    }

    [Theory, InlineAutoMoqData("./TestInputs/ValidAssetProjectInformation.json")]
    public async Task WhenShouldBeProcessedTrue_AssetAddedToProcessList(string jsonPath,Mock<IAssetProjectInfoValidator> assetProjectInfoValidatorMock)
    {
        assetProjectInfoValidatorMock.Setup(m => m.ShouldBeProcessed(It.IsAny<DateTimeOffset>(),It.IsAny<AssetProjectInfo>()))
            .Returns(true);
        var sut = SutWithFakeHandler(jsonPath, assetProjectInfoValidatorMock.Object);
        var res = await sut.FetchInitialLoadAsync();
        res.ProjectsToProcess.Count.Should().Be(1);
        res.DiscardedProjects.Should().BeEmpty();
  }
    
    [Theory, InlineAutoMoqData("./TestInputs/ValidAssetProjectInformation.json")]
    public async Task WhenShouldBeProcessedFalse_AssetAddedToDiscardedList(string jsonPath,Mock<IAssetProjectInfoValidator> assetProjectInfoValidatorMock)
    {
        assetProjectInfoValidatorMock.Setup(m => m.ShouldBeProcessed(It.IsAny<DateTimeOffset>(),It.IsAny<AssetProjectInfo>()))
            .Returns(false);
        var sut = SutWithFakeHandler(jsonPath, assetProjectInfoValidatorMock.Object);
        var res = await sut.FetchInitialLoadAsync();
        res.DiscardedProjects.Count.Should().Be(1);
        res.ProjectsToProcess.Should().BeEmpty();
    }
   
    private static InitialLoadService SutWithFakeHandler(string jsonPath, IAssetProjectInfoValidator assetProjectInfoValidator)
    {
        IConfiguration configuration = TestHelper.ConfigureInMemory("https://test.be", "2000-04-05");
        var fileContent = TestHelper.GetFileContent(jsonPath);
        HttpClient httpClient = new HttpClient(TestHelper.GetFakeHandler(HttpStatusCode.Accepted,fileContent));
        return new InitialLoadService(httpClient, assetProjectInfoValidator, configuration);

    }

}