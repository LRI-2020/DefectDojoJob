using System.Net;
using DefectDojoJob.Tests.Helpers.Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnectorTests;

public class UpdateMetadataAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdate_URiIsCorrect(IConfiguration configuration,Metadata metadata)
    {
        var fakeHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(metadata));
        var httpclient = new HttpClient(fakeHandler);
        httpclient.BaseAddress = new Uri("https://www.test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpclient);

        await sut.UpdateMetadataAsync(metadata);

        fakeHandler.RequestUrl.Should().NotBeNull();
        fakeHandler.RequestUrl!.AbsolutePath.Should().Be($"/metadata/{metadata.Id}");
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdate_BodySentIsCorrect(IConfiguration configuration,Metadata metadata)
    {
        var fakeHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(metadata));
        var httpclient = new HttpClient(fakeHandler);
        httpclient.BaseAddress = new Uri("https://www.test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpclient);

        await sut.UpdateMetadataAsync(metadata);

        fakeHandler.RequestBody.Should().NotBeNull();
        var actualBody = JObject.Parse(fakeHandler.RequestBody ?? "");
        ((string?)actualBody["value"]).Should().Be(metadata.Value);
        ((string?)actualBody["name"]).Should().Be(metadata.Name);
        ((int?)actualBody["product"]).Should().Be(metadata.Product);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateOk_UpdatedMetadataReturned(IConfiguration configuration,Metadata metadata)
    {
        var fakeHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(metadata));
        var httpclient = new HttpClient(fakeHandler);
        httpclient.BaseAddress = new Uri("https://www.test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpclient);

        var res = await sut.UpdateMetadataAsync(metadata);

        res.Should().NotBeNull();
        res.Should().BeOfType(typeof(Metadata));
    }
    
    [Theory]
    [InlineAutoMoqData(404,"No metadata with Id")]
    [InlineAutoMoqData(401,"Error while updating the metadata")]
    public async Task WhenErrorDuringUpdate_ErrorThrown(int errorCode,string errorMessage, IConfiguration configuration,Metadata metadata)
    {
        var fakeHandler = TestHelper.GetFakeHandler((HttpStatusCode)errorCode, JsonConvert.SerializeObject(metadata));
        var httpclient = new HttpClient(fakeHandler);
        httpclient.BaseAddress = new Uri("https://www.test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpclient);

        Func<Task> act = () => sut.UpdateMetadataAsync(metadata);

        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains(errorMessage));
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenErrorWhenParsingResponse_ErrorThrown(IConfiguration configuration,Metadata metadata)
    {
        var fakeHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(""));
        var httpclient = new HttpClient(fakeHandler);
        httpclient.BaseAddress = new Uri("https://www.test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpclient);

        Func<Task> act = () => sut.UpdateMetadataAsync(metadata);

        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains($"Updated Metadata '{metadata.Name}' linked to product id '{metadata.Product}' could not be retrieved"));
    }
}