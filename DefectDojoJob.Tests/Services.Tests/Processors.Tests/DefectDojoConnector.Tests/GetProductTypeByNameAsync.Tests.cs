using System.Globalization;
using System.Net;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests.DefectDojoConnector.Tests;

public class GetProductTypeByNameAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenGetProductTypeByName_URiIsCorrect(IConfiguration configuration, string name)
    {
        //Arrange
        var res = new ProductType(1, new DateTime(), new DateTime(), name);
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.GetProductTypeByNameAsync(name);

        //Assert
        var expectedAbsolutePath = "/product_types/";
        var expectedQuery = $"?name={name}";

        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.Query.Should().BeEquivalentTo(expectedQuery);
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenSuccessful_ReturnProductType(IConfiguration configuration, string name, DateTime created, DateTime updated)
    {
        //Arrange
 
        var apiResponse = $@"{{
           ""count"": 7,
            ""next"": null,
            ""previous"": null,
            ""results"": [
            {{
            ""id"": 1,
            ""name"" :""{name}"",
            ""updated"": ""{updated.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")}"",
            ""created"": ""{created.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")}"",
        }}]
        }}";
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, apiResponse);
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.GetProductTypeByNameAsync(name);
        var expectedRes = new ProductType(1,updated,created,name);

        //Assert
        actualRes.Should().BeEquivalentTo(expectedRes);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenStatusUnsuccessful_ErrorThrown(IConfiguration configuration, string name)
    {
        //Arrange
        var res = new ProductType(1,new DateTime(),new DateTime(),name);
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Forbidden, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.GetProductTypeByNameAsync(name);

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains("Error while processing the request")
                        && e.Message.Contains(HttpStatusCode.Forbidden.ToString()));
    }
}