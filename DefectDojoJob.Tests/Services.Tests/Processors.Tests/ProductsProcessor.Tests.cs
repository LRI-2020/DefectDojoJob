
namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests;

public class ProductsProcessorTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateCallWithoutProductId_ErrorAdded([Frozen] Mock<IProductExtractor> productExtractorMock,
        ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi, Product product)
    {
        productExtractorMock.Setup(m => m.ExtractProduct(It.IsAny<AssetProject>(), It.IsAny<List<AssetToDefectDojoMapper>>()))
            .ReturnsAsync(product);
        var res = await sut.ProcessProductAsync(pi, users, ProductAdapterAction.Update);
        res.Entity.Should().BeNull();
        res.Errors.Should().Contain(e => e.Message.Contains("no productId"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenInvalidAction_ErrorAdded(ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        var res = await sut.ProcessProductAsync(pi, users, ProductAdapterAction.None);
        res.Entity.Should().BeNull();
            res.Errors.Should().Contain(e=>e.Message.Contains("Invalid action"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenWarning_AddedToResult([Frozen] Mock<IProductExtractor> productExtractorMock,
        ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        productExtractorMock.Setup(m => m.ExtractProduct(It.IsAny<AssetProject>(), It.IsAny<List<AssetToDefectDojoMapper>>()))
            .ThrowsAsync(new WarningAssetProjectProcessor());
        var res = await sut.ProcessProductAsync(pi, users, ProductAdapterAction.Create);
        res.Entity.Should().BeNull();
        res.Warnings.Count.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenValidCreateRequired_CreateCalled(Product product,
        [Frozen] Mock<IProductExtractor> productExtractorMock,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        productExtractorMock.Setup(m => m.ExtractProduct(It.IsAny<AssetProject>(), It.IsAny<List<AssetToDefectDojoMapper>>()))
            .ReturnsAsync(product);
         await sut.ProcessProductAsync(pi, users, ProductAdapterAction.Create);
        defectDojoConnectorMock.Verify(m=>m.CreateProductAsync(product),Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenValidUpdateRequired_UpdateCalled(Product product,
        [Frozen] Mock<IProductExtractor> productExtractorMock,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        productExtractorMock.Setup(m => m.ExtractProduct(It.IsAny<AssetProject>(), It.IsAny<List<AssetToDefectDojoMapper>>()))
            .ReturnsAsync(product);
        await sut.ProcessProductAsync(pi, users, ProductAdapterAction.Update,product.Id);
        defectDojoConnectorMock.Verify(m=>m.UpdateProductAsync(product),Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenCreateOk_AssetMapperAddedToRes(Product product,Metadata metadata, ProductType productType,
        [Frozen] Mock<IProductExtractor> productExtractorMock,
        [Frozen] MockDefectDojoConnector defectDojoConnectorMock,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        defectDojoConnectorMock.DefaultCreateSetup(product, metadata, productType);
        productExtractorMock.Setup(m => m.ExtractProduct(It.IsAny<AssetProject>(), It.IsAny<List<AssetToDefectDojoMapper>>()))
            .ReturnsAsync(product);
        var sut = new ProductsProcessor(defectDojoConnectorMock.Object,productExtractorMock.Object);

        var res = await sut.ProcessProductAsync(pi, users, ProductAdapterAction.Create);
        res.Entity.Should().NotBeNull();
        res.Entity!.AssetIdentifier.Should().Be(pi.Code);
        res.Entity.DefectDojoId.Should().Be(product.Id);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateOk_AssetMapperAddedToRes(ProductType productType, Product product, Metadata metadata,
        [Frozen] Mock<IProductExtractor> productExtractorMock,
        [Frozen] MockDefectDojoConnector defectDojoConnectorMock,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        defectDojoConnectorMock.DefaultUpdateSetup(product, metadata, productType);
        productExtractorMock.Setup(m => m.ExtractProduct(It.IsAny<AssetProject>(), It.IsAny<List<AssetToDefectDojoMapper>>()))
            .ReturnsAsync(product);
        var sut = new ProductsProcessor(defectDojoConnectorMock.Object,productExtractorMock.Object);

        var res = await sut.ProcessProductAsync(pi, users, ProductAdapterAction.Update,product.Id);
        res.Entity.Should().NotBeNull();
        res.Entity!.AssetIdentifier.Should().Be(pi.Code);
        res.Entity.DefectDojoId.Should().Be(product.Id);
    }
}