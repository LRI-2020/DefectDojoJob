using System.Data.Common;
using System.Net;
using DefectDojoJob.Services.Connectors;
using DefectDojoJob.Tests.Helpers.Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnectorTests;

public class CreateMetadataAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenCreateOK_MetadataReturned(IConfiguration configuration, Metadata metadata)
    {
        var fakeHttpMessageHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(metadata));
        var httpClient = new HttpClient(fakeHttpMessageHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        var res = await sut.CreateMetadataAsync(metadata);
        res.Should().NotBeNull();
        res.Should().BeOfType(typeof(Metadata));
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenCreateMetadata_UriIsCorrect(IConfiguration configuration, Metadata metadata)
    {
        var fakeHttpMessageHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(metadata));
        var httpClient = new HttpClient(fakeHttpMessageHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        await sut.CreateMetadataAsync(metadata);
        var expectedAbsolutePath = "/metadata/";
        fakeHttpMessageHandler.RequestUrl.Should().NotBeNull();
        fakeHttpMessageHandler.RequestUrl?.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenCreate_BodySentIsCorrect(IConfiguration configuration, Metadata metadata)
    {
        var fakeHttpMessageHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(metadata));
        var httpClient = new HttpClient(fakeHttpMessageHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        await sut.CreateMetadataAsync(metadata);
        
        fakeHttpMessageHandler.RequestBody.Should().NotBeNull();
        
        var actualBody = JObject.Parse(fakeHttpMessageHandler.RequestBody ?? "");
        ((string?)actualBody["value"]).Should().Be(metadata.Value);
        ((string?)actualBody["name"]).Should().Be(metadata.Name);
        ((int?)actualBody["product"]).Should().Be(metadata.Product);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenStatusCodeNotSuccess_ErrorThrown(IConfiguration configuration, Metadata metadata)
    {
        var fakeHttpMessageHandler = TestHelper.GetFakeHandler(HttpStatusCode.Forbidden);
        var httpClient = new HttpClient(fakeHttpMessageHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        Func<Task> act = ()=> sut.CreateMetadataAsync(metadata);
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains("Error while creating the Metadata"));
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenResCannotBeParsed_ErrorThrown(IConfiguration configuration, Metadata metadata)
    {
        var fakeHttpMessageHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted,JsonConvert.SerializeObject(new{}));
        var httpClient = new HttpClient(fakeHttpMessageHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        Func<Task> act = ()=> sut.CreateMetadataAsync(metadata);

        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains($"New Metadata '{metadata.Name}' could not be retrieved"));
    }
}