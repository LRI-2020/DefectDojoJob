using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Tests.Services.Tests.Adapters.Tests.ProjectsAdapter.Tests;

public class StartAdapterAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenSeveralProjects_AllAreAdapted(DefectDojoJob.Services.Adapters.ProjectsAdapter sut, List<AssetProject> projects,
        List<AssetToDefectDojoMapper> users)
    {
        var res = await sut.StartAdapterAsync(projects, users);
        res.Count.Should().Be(projects.Count);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenErrorOccurred_ErrorAddedToResult([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ErrorAssetProjectProcessor error,
        DefectDojoJob.Services.Adapters.ProjectsAdapter sut,
        AssetProject project,
        List<AssetToDefectDojoMapper> users)
    {
        defectDojoConnectorMock.Setup(m =>
            m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ThrowsAsync(error);
        var projects = new List<AssetProject>() { project };
        var res = await sut.StartAdapterAsync(projects, users);
        res.Count.Should().Be(1);
        res[0].Errors.Count.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenWarningOccurred_WarningAddedToResult([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        WarningAssetProjectProcessor warning,
        DefectDojoJob.Services.Adapters.ProjectsAdapter sut,
        AssetProject project,
        List<AssetToDefectDojoMapper> users)
    {
        defectDojoConnectorMock.Setup(m =>
            m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ThrowsAsync(warning);
        var projects = new List<AssetProject>() { project };
        var res = await sut.StartAdapterAsync(projects, users);
        res.Count.Should().Be(1);
        res[0].Warnings.Count.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenWarningOrError_ProcessingContinue([Frozen] Mock<IProductsProcessor> productProcessorMock,
        DefectDojoJob.Services.Adapters.ProjectsAdapter sut, ErrorAssetProjectProcessor error, WarningAssetProjectProcessor warning,
        List<AssetToDefectDojoMapper> users, ProductProcessingResult result)
    {
        var projects = new Fixture().CreateMany<AssetProject>(4).ToList();
        productProcessorMock.Setup(m => m.ProcessProductAsync(It.IsAny<AssetProject>(),
                It.IsAny<List<AssetToDefectDojoMapper>>(), It.IsAny<ProductAdapterAction>(), It.IsAny<int?>()))
            .ReturnsAsync(result);
        productProcessorMock.Setup(m => m.ProcessProductAsync(projects[0],
                It.IsAny<List<AssetToDefectDojoMapper>>(), It.IsAny<ProductAdapterAction>(), It.IsAny<int?>()))
            .Throws(error);
        productProcessorMock.Setup(m => m.ProcessProductAsync(projects[1],
                It.IsAny<List<AssetToDefectDojoMapper>>(), It.IsAny<ProductAdapterAction>(), It.IsAny<int?>()))
            .Throws(warning);
        var results = await sut.StartAdapterAsync(projects, users);

        results.Count.Should().Be(projects.Count);
    }
}