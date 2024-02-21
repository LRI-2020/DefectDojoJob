using System.Net;
using DefectDojoJob.Tests.Helpers.Tests;
using Newtonsoft.Json;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnector.Tests;

public class GetMetadataAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenGetMetadata_URiIsCorrect(IConfiguration configuration, string key, string value, Metadata res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.GetMetadataAsync(new Dictionary<string, string>() { { key, value } });

        //Assert
        var expectedAbsolutePath = "/metadata/";
        var expectedQuery = $"?{key}={value}";

        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.Query.Should().BeEquivalentTo(expectedQuery);
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenSuccessful_ReturnMetadata(IConfiguration configuration, Metadata res)
    {
        //Arrange

        var apiResponse = $@"{{
           ""count"": 7,
            ""next"": null,
            ""previous"": null,
            ""results"": [
            {{
            ""id"": {res.Id},
            ""product"" :""{res.Product}"",
            ""name"": ""{res.Name}"",
            ""value"": ""{res.Value}"",
        }}]
        }}";
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, apiResponse);
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.GetMetadataAsync(new Dictionary<string, string>());

        //Assert
        actualRes.Should().BeEquivalentTo(res);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenStatusUnsuccessfulButNot404_ErrorThrown(IConfiguration configuration, Metadata res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Forbidden, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.GetMetadataAsync(new Dictionary<string, string>());

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains("Error while processing the request")
                        && e.Message.Contains(HttpStatusCode.Forbidden.ToString()));
    }

    [Theory]
    [AutoMoqData]
    public async Task When404_ReturnNull(IConfiguration configuration, Metadata res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.NotFound, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.GetMetadataAsync(new Dictionary<string, string>());

        //Assert
        actualRes.Should().BeNull();
    }
}