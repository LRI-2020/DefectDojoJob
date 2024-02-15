using System.Net;
using System.Text;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace DefectDojoJob.Tests.Services.Tests;

public class DefectDojoConnectorTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenCreate_ArgumentsMappedCorrectlyToBody(IConfiguration configuration,string name,string description)
    {
        //Arrange
        var res = new Product("test", "test") { Id = 1 };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoConnector(configuration,httpClient);

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
 
        var actualBody = JsonConvert.DeserializeObject(fakeHttpHandler.requestBody??"");
        var expected = JObject.Parse(JsonConvert.SerializeObject(expectedBody));
        actualBody.Should().BeEquivalentTo(expected);
    }
}