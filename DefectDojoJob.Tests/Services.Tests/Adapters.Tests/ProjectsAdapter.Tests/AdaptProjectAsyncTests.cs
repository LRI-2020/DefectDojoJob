namespace DefectDojoJob.Tests.Services.Tests.Adapters.Tests.ProjectsAdapter.Tests;

public class AdaptProjectAsyncTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenCodeFoundButNotProject_ErrorThrown(Metadata metadataRes,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Adapters.ProjectsAdapter sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(metadataRes);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((Product?)null);

        Func<Task> act = () => sut.AdaptProjectAsync(pi, users);

        await act.Should().ThrowAsync<ErrorAssetProjectProcessor>()
            .Where(e => e.Message.Contains("Mismatch Code and Name"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNameFoundButNotCode_ErrorThrown(Product productRes,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Adapters.ProjectsAdapter sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync((Metadata?)null);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(productRes);

        Func<Task> act = () => sut.AdaptProjectAsync(pi, users);

        await act.Should().ThrowAsync<ErrorAssetProjectProcessor>()
            .Where(e => e.Message.Contains("Mismatch Code and Name"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenMetadataAndProductIdDifferent_ErrorThrown(Product productRes, Metadata metadataRes,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Adapters.ProjectsAdapter sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        metadataRes.Product = 1;
        productRes.Id = 2;

        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(metadataRes);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(productRes);

        Func<Task> act = () => sut.AdaptProjectAsync(pi, users);

        await act.Should().ThrowAsync<ErrorAssetProjectProcessor>()
            .Where(e => e.Message.Contains("Mismatch Code and Name")
                        && e.Message.Contains("not linked together"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenExistingProject_UpdateIsCalled(Product product, Metadata metadata, ProductType productType,
        Mock<IProductsProcessor> productsProcessorMock,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        throw new NotImplementedException();
        metadata.Product = 1;
        product.Id = 1;
        var defectDojoConnectorMock = new MockDefectDojoConnector().DefaultUpdateSetup(product, metadata, productType);
    //    var sut = new DefectDojoJob.Services.Adapters.ProjectsAdapter(productsProcessorMock.Object,defectDojoConnectorMock.Object);
      //  await sut.AdaptProjectAsync(pi, users);

        productsProcessorMock.Verify(m => m.ProcessProductAsync(It.IsAny<AssetProject>(),
            It.IsAny<List<AssetToDefectDojoMapper>>(), ProductAdapterAction.Update,
            It.IsAny<int?>()), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNotExistingProject_CreateIsCalled(ProductType productType,
        Product product,
        Metadata metadata,
        Mock<IProductsProcessor> productsProcessorMock,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        throw new NotImplementedException();

        var defectDojoConnectorMock = new MockDefectDojoConnector().DefaultCreateSetup(product, metadata, productType);
       // var sut = new DefectDojoJob.Services.Adapters.ProjectsAdapter(productsProcessorMock.Object,defectDojoConnectorMock.Object);
        //await sut.AdaptProjectAsync(pi, users);
        productsProcessorMock.Verify(m => m.ProcessProductAsync(It.IsAny<AssetProject>(),
            It.IsAny<List<AssetToDefectDojoMapper>>(), ProductAdapterAction.Create,null), Times.Once);
    }
}