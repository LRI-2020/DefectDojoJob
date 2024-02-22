using System.Net;
using DefectDojoJob.Tests.Helpers.Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnectorTests;

public class CreateProductAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenCreate_UriIsCorrect(IConfiguration configuration)
    {
        //Arrange
        var res = new Product("test", "test") { Id = 1 };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.CreateProductAsync(res);

        //Assert
        //Assert
        var expectedAbsolutePath = "/products/";
        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenCreate_ArgumentsMappedCorrectlyToBody(IConfiguration configuration, string name, string description)
    {
        //Arrange
        var res = new Product("test", "test") { Id = 1 };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        var productToCreate = new Product(name, description)
        {
            ProductTypeId = 1,
            Lifecycle = Lifecycle.construction,
            TechnicalContact = 2,
            TeamManager = 3,
            ProductManager = 4,
            UserRecords = 5,
            ExternalAudience = true
        };
        //Act
        await sut.CreateProductAsync(productToCreate);

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
    public async Task WhenSuccessful_ReturnProductCreated(IConfiguration configuration, Product res)
    {
        //Arrange

        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.CreateProductAsync(res);

        //Assert
        actualRes.Should().BeOfType(typeof(Product));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenStatusUnsuccessful_ErrorThrown(IConfiguration configuration, Product res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Forbidden, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.CreateProductAsync(res);

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains("Error while creating the Project")
                        && e.Message.Contains(HttpStatusCode.Forbidden.ToString()));
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenResCannotBeParsed_ErrorThrown(IConfiguration configuration, Product product)
    {
        var fakeHttpMessageHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted);
        var httpClient = new HttpClient(fakeHttpMessageHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.Connectors.DefectDojoConnector(configuration, httpClient);

        Func<Task> act = ()=> sut.CreateProductAsync(product);

        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains($"New Product '{product.Name}' could not be retrieved"));
    }
}