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

public class UpdateProductAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdate_ArgumentsMappedCorrectlyToBody(IConfiguration configuration,string name,string description)
    {
        //Arrange
        var res = new Product("test", "test") { Id = 1 };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration,httpClient);

        var productToUpdate = new Product(name, description)
        {
            Id=1,
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
        var expectedBody =  new
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
 
        var actualBody = JsonConvert.DeserializeObject(fakeHttpHandler.RequestBody??"");
        var expected = JObject.Parse(JsonConvert.SerializeObject(expectedBody));
        actualBody.Should().BeEquivalentTo(expected);
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
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

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
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.UpdateProductAsync(res);

        //Assert
        actualRes.Should().BeOfType(typeof(Product));
    }
    
    [Theory]
    [InlineAutoMoqData(404,"No product with Id")]
    [InlineAutoMoqData(403,"Error while updating the Project")]
    public async Task WhenStatusUnsuccessful_ErrorThrown(int statusCode, string error, IConfiguration configuration, Product res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler((HttpStatusCode)statusCode, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.UpdateProductAsync(res);

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains(error));
    }
    
   
}