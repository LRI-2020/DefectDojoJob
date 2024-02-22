using System.Net;
using DefectDojoJob.Tests.Helpers.Tests;
using Newtonsoft.Json;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnector.Tests;

public class DeleteProductAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenDeleteProduct_URiIsCorrect(IConfiguration configuration, int id)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(""));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.DeleteProductAsync(id);

        //Assert
        var expectedAbsolutePath = $"/products/{id}";

        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }


}