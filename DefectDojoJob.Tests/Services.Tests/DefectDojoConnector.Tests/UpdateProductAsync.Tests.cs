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
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration,httpClient);

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

}