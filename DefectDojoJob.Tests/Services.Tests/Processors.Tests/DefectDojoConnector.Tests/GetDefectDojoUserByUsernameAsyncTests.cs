﻿using System.Net;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests.DefectDojoConnector.Tests;

public class GetDefectDojoUserByUsernameAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenGetUserByUsername_URiIsCorrect(IConfiguration configuration, string name)
    {
        //Arrange
        var res = new User { UserName = name };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

        //Act
        await sut.GetDefectDojoUserByUsernameAsync(name);

        //Assert
        var expectedAbsolutePath = "/users/";
        var expectedQuery = $"?username={name}";

        var actualUri = fakeHttpHandler.RequestUrl ?? new Uri("");
        actualUri.Query.Should().BeEquivalentTo(expectedQuery);
        actualUri.AbsolutePath.Should().BeEquivalentTo(expectedAbsolutePath);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenSuccessful_ReturnUser(IConfiguration configuration, string name)
    {
        //Arrange
        var apiResponse = $@"{{
           ""count"": 7,
            ""next"": null,
            ""previous"": null,
            ""results"": [
            {{
            ""id"": 1,
            ""username"" :""{name}""
        }}]
        }}";
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Accepted, apiResponse);
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

        //Act
        var actualRes = await sut.GetDefectDojoUserByUsernameAsync(name);
        var expectedRes = new User() { Id = 1, UserName = name };

        //Assert
        actualRes.Should().BeEquivalentTo(expectedRes);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenStatusUnsuccessful_ErrorThrown(IConfiguration configuration, string name)
    {
        //Arrange
        var res = new User { UserName = name };
        var fakeHttpHandler = TestHelper.GetFakeHandler(HttpStatusCode.Forbidden, JsonConvert.SerializeObject(res));
        var httpClient = new HttpClient(fakeHttpHandler);
        httpClient.BaseAddress = new Uri("https://test.be");
        var sut = new DefectDojoJob.Services.DefectDojoConnector(configuration, httpClient);

        //Act
        Func<Task> act = () => sut.GetDefectDojoUserByUsernameAsync(name);

        //Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.Contains("Error while processing the request")
                        && e.Message.Contains(HttpStatusCode.Forbidden.ToString()));
    }
}