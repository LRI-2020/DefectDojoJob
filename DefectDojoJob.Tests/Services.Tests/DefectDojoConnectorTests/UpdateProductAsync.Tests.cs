using System.Net;
using DefectDojoJob.Tests.Helpers.Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnectorTests;

public class UpdateProductAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdate_ArgumentsMappedCorrectlyToBody(IConfiguration configuration, string name, string description)
    {
        //Arrange
        var res = new Product("test", "test") { Id = 1 };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        var productToUpdate = new Product(name, description)
        {
            Id = 1,
            ProductTypeId = 1,
            Lifecycle = Lifecycle.construction,
            TechnicalContact = 2,
            TeamManager = 3,
            ProductManager = 4,
            UserRecords = 5,
            ExternalAudience = true
        };
        //Act
        await sut.UpdateProductAsync(productToUpdate);

        //Assert
        var actualBody = JObject.Parse(fakeHttpHandler.RequestBody ?? "");
        ((string?)actualBody["name"]).Should().Be(name);
        ((string?)actualBody["description"]).Should().Be(description);
        ((int?)actualBody["prod_type"]).Should().Be(1);
        ((int?)actualBody["technical_contact"]).Should().Be(2);
        ((int?)actualBody["team_manager"]).Should().Be(3);
        ((int?)actualBody["product_manager"]).Should().Be(4);
        ((int?)actualBody["user_records"]).Should().Be(5);
        ((bool?)actualBody["external_audience"]).Should().Be(true);
        ((string?)actualBody["lifecycle"]).Should().Be(Lifecycle.construction.ToString());
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdate_UriIsCorrect(IConfiguration configuration)
    {
        //Arrange
        var res = new Product("test", "test") { Id = 1 };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.UpdateProductAsync(res);

        //Assert
        //Assert
        var expectedAbsolutePath = $"/products/{res.Id}";
        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenSuccessful_ReturnProductUpdated(IConfiguration configuration, Product res)
    {
        //Arrange

        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.UpdateProductAsync(res);

        //Assert
        actualRes.Should().BeOfType(typeof(Product));
    }

    [Theory]
    [InlineAutoMoqData(404, "No product with Id")]
    [InlineAutoMoqData(403, "Error while updating the Project")]
    public async Task WhenStatusUnsuccessful_ErrorThrown(int statusCode, string error, IConfiguration configuration, Product res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler((HttpStatusCode)statusCode, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.UpdateProductAsync(res);

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains(error));
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenResCannotBeParsed_ErrorThrown(IConfiguration configuration, Product product)
    {
        var fakeHttpMessageHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted);
        var httpClient = new HttpClient(fakeHttpMessageHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        Func<Task> act = ()=> sut.UpdateProductAsync(product);

        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message
                .Contains($"Updated Product '{product.Name}' could not be retrieved"));
    }
}