using AutoFixture.Xunit2;
using DefectDojoJob.Models.Processor.Interfaces;
using DefectDojoJob.Services;
using DefectDojoJob.Tests.AutoDataAttribute;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DefectDojoJob.Tests;

public class FetchInitialLoadAsyncTests
{
    [Theory]
    [InlineAutoMoqData("test","2020-03-02")]
    public async Task Test1(string url,string refDate,HttpClient httpClient,Mock<IAssetProjectInfoValidator> assetProjectInfoValidator)
    {
        IConfiguration configuration = SetupConfiguration(url,refDate);
        var sut = new InitialLoadService(httpClient, assetProjectInfoValidator.Object, configuration);
        await sut.FetchInitialLoadAsync();
    }

    private IConfiguration SetupConfiguration(string url, string refDate)
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"AssetUrl", url},
            {"LastRunDate", refDate}
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
}