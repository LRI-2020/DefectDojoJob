using System.Net;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DefectDojoJob.Tests.Services.Tests.DefectDojoConnector.Tests;

public class GetProductByNameAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenGetProductByName_URiIsCorrect(IConfiguration configuration, string name, Product res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.GetProductByNameAsync(name);

        //Assert
        var expectedAbsolutePath = "/products/";
        var expectedQuery = $"?name={name}";

        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.Query.Should().BeEquivalentTo(expectedQuery);
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenSuccessful_ReturnProduct(IConfiguration configuration, string name, string description,int id,int type)
    {
        //Arrange
        var res = new Product(name, description) { Id = id, ProductTypeId = type };
        var apiResponse = $@"{{
           ""count"": 7,
            ""next"": null,
            ""previous"": null,
            ""results"": [
            {{
            ""id"": {res.Id},
            ""description"" :""{res.Description}"",
            ""name"": ""{res.Name}"",
            ""prod_type"":""{res.ProductTypeId}""
        }}]
        }}";
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, apiResponse);
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.GetProductByNameAsync(name);

        //Assert
        actualRes.Should().BeEquivalentTo(res);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenStatusUnsuccessfulButNot404_ErrorThrown(IConfiguration configuration, string name, Product res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Forbidden, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.GetProductByNameAsync(name);

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains("Error while processing the request")
                        && e.Message.Contains(HttpStatusCode.Forbidden.ToString()));
    }

    [Theory]
    [AutoMoqData]
    public async Task When404_ReturnNull(IConfiguration configuration, string name, Product res)
    {
        //Arrange
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.NotFound, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnectors.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.GetProductByNameAsync(name);

        //Assert
        actualRes.Should().BeNull();
    }
}