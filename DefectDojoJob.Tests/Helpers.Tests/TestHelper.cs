using System.Net;
using Microsoft.Extensions.Configuration;

namespace DefectDojoJob.Tests.Helpers.Tests;

public static class TestHelper
{
    public static IConfiguration ConfigureInMemory(string url, string refDate)
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AssetUrl", url },
            { "LastRunDate", refDate }
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    public static IConfiguration ConfigureJsonFile(string jsonPath)
    {
        return new ConfigurationBuilder()
            .AddJsonFile(jsonPath)
            .Build();
    }

    public static FakeHttpMessageHandler GetFakeHandler(HttpStatusCode statusCode, string? responseJson=null )
    {
       return new FakeHttpMessageHandler(statusCode, responseJson);
    }

    public static string GetFileContent(string jsonPath)
    {
        using StreamReader reader = new(jsonPath);
        return reader.ReadToEnd();
    }
    
    

}