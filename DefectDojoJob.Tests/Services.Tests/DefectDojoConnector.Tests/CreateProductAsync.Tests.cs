using System.Net;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnector.Tests;

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
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

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
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

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
        var expectedBody = new
        {
            name,
            description,
            prod_type = 1,
            technical_contact = 2,
            team_manager = 3,
            product_manager = 4,
            user_records = 5,
            external_audience = true,
            lifecycle = Lifecycle.construction
        };

        var actualBody = JsonConvert.DeserializeObject(fakeHttpHandler.RequestBody ?? "");
        var expected = JObject.Parse(JsonConvert.SerializeObject(expectedBody));
        actualBody.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenSuccessful_ReturnProductCreated(IConfiguration configuration, Product res)
    {
        //Arrange

        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

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
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.CreateProductAsync(res);

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains("Error while creating the Project")
                        && e.Message.Contains(HttpStatusCode.Forbidden.ToString()));
    }
}