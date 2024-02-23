using System.Net;
using DefectDojoJob.Tests.Helpers.Tests;
using Newtonsoft.Json;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnectorTests;

public class DeleteMetadataAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenDeleteMetadata_URiIsCorrect(IConfiguration configuration, int id)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted);
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.DeleteMetadataAsync(id);

        //Assert
        var expectedAbsolutePath = $"/metadata/{id}";

        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenDeleteOk_TrueReturned(IConfiguration configuration, int id)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted);
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var res = await sut.DeleteMetadataAsync(id);

        //Assert
        res.Should().BeTrue();
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenDeleteFailed_FalseReturned(IConfiguration configuration, int id)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Forbidden);
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var res = await sut.DeleteMetadataAsync(id);

        //Assert
        res.Should().BeFalse();
    }
}