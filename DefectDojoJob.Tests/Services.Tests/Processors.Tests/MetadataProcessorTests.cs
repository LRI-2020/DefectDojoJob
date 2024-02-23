namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests;

public class MetadataProcessorTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenActionCreateRequired_CreateIsCalled(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockCreateMetadataAsync(metadata);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);
        await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        defectDojoConnectorMock.Verify(m => m.CreateMetadataAsync(metadata), Times.Once);
    }
    [Theory]
    [AutoMoqData]
    public async Task WhenCreateOk_MetadataAddedToRes(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockCreateMetadataAsync(metadata);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().Contain(r => r.DefectDojoId == metadata.Id);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateOk_MetadataAddedToRes(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockUpdateMetadataAsync(metadata);
        defectDojoConnectorMock.MockGetMetadataAsync(metadata);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Update, productId);
        res.Entities.Should().Contain(r => r.DefectDojoId == metadata.Id);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenProcess_AllMetadataProcessed([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        [Frozen] Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata1, Metadata metadata2, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata1, false), (metadata2, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata1)).ReturnsAsync(metadata1);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata2)).ReturnsAsync(metadata2);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().Contain(r => r.DefectDojoId == metadata1.Id);
        res.Entities.Should().Contain(r => r.DefectDojoId == metadata2.Id);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateProjectAndMetadataDoesNotExist_MetadataCreated(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockGetMetadataAsync(null);
        defectDojoConnectorMock.MockCreateMetadataAsync(metadata);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Update, productId);

        res.Entities.Should().Contain(r => r.DefectDojoId == metadata.Id);
        defectDojoConnectorMock.Verify(m => m.CreateMetadataAsync(metadata), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateProjectAndMetadataExist_MetadataUpdated(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockGetMetadataAsync(metadata);
        defectDojoConnectorMock.MockUpdateMetadataAsync(metadata);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Update, productId);

        res.Entities.Should().Contain(r => r.DefectDojoId == metadata.Id);
        defectDojoConnectorMock.Verify(m => m.UpdateMetadataAsync(metadata), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateProject_AssetCodeIsNotProcessed(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata originalMetadata, Metadata metadataWithNewCode, int productId)
    {
        originalMetadata.Name = "assetCode";
        metadataWithNewCode.Name = "assetCode";
        List<(Metadata, bool)> extraction = new() { (metadataWithNewCode, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockGetMetadataAsync(originalMetadata);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Update, productId);

        res.Entities.Should().Contain(r => r.DefectDojoId == originalMetadata.Id);
        defectDojoConnectorMock.Verify(m => m.UpdateMetadataAsync(It.IsAny<Metadata>()), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateProject_AssetCodeShouldAlreadyExistInMetadata(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
        metadata.Name = "assetCode";
        List<(Metadata, bool)> extraction = new() { (metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockGetMetadataAsync(null);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Update, productId);

        res.Entities.Should().BeEmpty();
        res.Errors.Should().Contain(e =>
            e.Message.Contains($"Project already exist in defect dojo but assetCode '{metadata.Value}' could not be found"));
        defectDojoConnectorMock.Verify(m => m.UpdateMetadataAsync(It.IsAny<Metadata>()), Times.Never);
        defectDojoConnectorMock.Verify(m => m.CreateMetadataAsync(It.IsAny<Metadata>()), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenActionIsCreateAndErrorOnRequiredMetadata_DeleteRelatedProduct(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId, Exception error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata)).ThrowsAsync(error);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        Func<Task> act = () => sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        await act.Should().ThrowAsync<Exception>();
        defectDojoConnectorMock.Verify(m => m.DeleteProductAsync(productId), Times.Once());
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenActionIsCreateAndErrorOnRequiredMetadata_DeleteMetadataAlreadyCreated(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata,Metadata metadata2, int productId, Exception error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata2,false),(metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata)).ThrowsAsync(error);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata2)).ReturnsAsync(metadata2);
        defectDojoConnectorMock.Setup(m => m.DeleteMetadataAsync(It.IsAny<int>())).ReturnsAsync(true);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

       await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);

        defectDojoConnectorMock.Verify(m => m.CreateMetadataAsync(It.IsAny<Metadata>()), Times.Exactly(2));
        defectDojoConnectorMock.Verify(m => m.DeleteProductAsync(productId), Times.Once());
        defectDojoConnectorMock.Verify(m => m.DeleteMetadataAsync(metadata2.Id), Times.Once());
    }
   
    [Theory]
    [AutoMoqData]
    public async Task WhenActionIsUpdateAndErrorOnRequiredMetadata_NoCompensation(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId, Exception error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata)).ThrowsAsync(error);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        var res = await  sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Update, productId);
        res.Errors.Count.Should().Be(1);
        defectDojoConnectorMock.Verify(m => m.DeleteProductAsync(productId), Times.Never());
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenProductCompensationSuccess_SpecificErrorAdded(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId, Exception error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.DeleteProductAsync(productId)).ReturnsAsync(true);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata)).ThrowsAsync(error);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        Func<Task> act = () => sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        await act.Should().ThrowAsync<Exception>().Where(e => e.Message.Contains(
            $"Metadata '{metadata.Name}' could not be created; Compensation successful- Product with Id '{productId}' with code {pi.Code} has been deleted"));
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenMetadataCompensationSuccess_SpecificErrorAdded(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata,Metadata metadata2, int productId, Exception error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata2,false),(metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata)).ThrowsAsync(error);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata2)).ReturnsAsync(metadata2);
        defectDojoConnectorMock.Setup(m => m.DeleteMetadataAsync(It.IsAny<int>())).ReturnsAsync(true);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);

        res.Errors.Should().Contain(e => e.Message.Contains(
            "Compensation successful for previously created metadata") && e.Message.Contains(metadata2.Id.ToString()));
        }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenMetadataCompensationFailed_SpecificErrorAdded(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata,Metadata metadata2, int productId, Exception error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata2,false),(metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata)).ThrowsAsync(error);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata2)).ReturnsAsync(metadata2);
        defectDojoConnectorMock.Setup(m => m.DeleteMetadataAsync(It.IsAny<int>())).ReturnsAsync(false);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);

        res.Errors.Should().Contain(e => e.Message.Contains(
            "Compensation failed for previously created metadata") && e.Message.Contains(metadata2.Id.ToString()));
    }

    
    [Theory]
    [AutoMoqData]
    public async Task WhenCompensationFailed_SpecificErrorThrown(MockDefectDojoConnector defectDojoConnectorMock,
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId, Exception error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, true) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata)).ThrowsAsync(error);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);

        Func<Task> act = () => sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        await act.Should().ThrowAsync<Exception>().Where(e => e.Message.Contains("Compensation has failed"));
    }
    
    [Theory]
    [InlineAutoMoqData("error")]
    public async Task WhenNonCriticalError_AddedToRes(string error, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        [Frozen] Mock<IMetadataExtractor> metadataExtractorMock, MetadataProcessor sut,
        AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(It.IsAny<Metadata>()))
            .ThrowsAsync(new Exception(error));
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().BeEmpty();
        res.Errors.Count.Should().Be(1);
        res.Errors.Should().Contain(e => e.Message == error);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenWarning_AddedToRes([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        [Frozen] Mock<IMetadataExtractor> metadataExtractorMock, MetadataProcessor sut,
        AssetProject pi, Metadata metadata, int productId, WarningAssetProjectProcessor warning)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(It.IsAny<Metadata>()))
            .ThrowsAsync(warning);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().BeEmpty();
        res.Warnings.Count.Should().Be(1);
        res.Warnings.Should().Contain(e => e.Message == warning.Message);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNonCriticalErrorOrWarning_ProcessContinue([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        [Frozen] Mock<IMetadataExtractor> metadataExtractorMock, MetadataProcessor sut,
        AssetProject pi, Metadata metadata1, Metadata metadata2, Metadata metadata3, int productId, WarningAssetProjectProcessor warning, ErrorAssetProjectProcessor error)
    {
        List<(Metadata, bool)> extraction = new() { (metadata1, false), (metadata2, false), (metadata3, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata1))
            .ThrowsAsync(warning);
        defectDojoConnectorMock.Setup(m => m.UpdateMetadataAsync(metadata2))
            .ThrowsAsync(error);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata3))
            .ReturnsAsync(metadata3);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().Contain(e => e.DefectDojoId == metadata3.Id);
        res.Warnings.Count.Should().Be(1);
        res.Errors.Count.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenInvalidActionRequired_ErrorInRes([Frozen]Mock<IMetadataExtractor> metadataExtractorMock,
        MetadataProcessor sut, AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata, bool)> extraction = new() { (metadata, false) };
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.None, productId);
        res.Errors.Should().Contain(e => e.Message.Contains(
            "Invalid action requested"));
    }
}