namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests;

public class MetadataProcessorTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenCreateOk_MetadataAddedToRes(MockDefectDojoConnector defectDojoConnectorMock, 
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
       List<(Metadata,bool)> extraction = new () { (metadata,false)};
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
       List<(Metadata,bool)> extraction = new () { (metadata,false)};
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
    public async Task WhenProcess_AllMetadataProcessed([Frozen]Mock<IDefectDojoConnector> defectDojoConnectorMock, 
       [Frozen] Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata1, Metadata metadata2, int productId)
    {
        List<(Metadata,bool)> extraction = new () { (metadata1,false), (metadata2,true)};
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m=>m.CreateMetadataAsync(metadata1)).ReturnsAsync(metadata1);
        defectDojoConnectorMock.Setup(m=>m.CreateMetadataAsync(metadata2)).ReturnsAsync(metadata2);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().Contain(r => r.DefectDojoId == metadata1.Id);
        res.Entities.Should().Contain(r => r.DefectDojoId == metadata2.Id);
    }   
    
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateButMetadataNotFound_SpecificError(MockDefectDojoConnector defectDojoConnectorMock, 
        Mock<IMetadataExtractor> metadataExtractorMock, AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata,bool)> extraction = new () { (metadata,false)};
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.MockUpdateMetadataAsync(metadata);
        defectDojoConnectorMock.MockGetMetadataAsync(null);
        var sut = new MetadataProcessor(defectDojoConnectorMock.Object, metadataExtractorMock.Object);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Update, productId);
        res.Errors.Should().Contain(r => r.Message == 
                                         $"No metadata '{metadata.Name}' linked to product with Id {metadata.Product} found, update could not be processed");
    }
    
   
    [Fact]
    public void WhenErrorDuringCreationOfRequiredMetadata_DeleteRelatedProduct(){
        throw new NotImplementedException();
    }
    
    [Fact]
    public void WhenErrorDuringCreationOfRequiredMetadata_ProcessIsStopped(){
        throw new NotImplementedException();
    }
    
    [Theory]
    [InlineAutoMoqData("error")]
    public async Task WhenNonCriticalError_AddedToRes(string error,[Frozen]Mock<IDefectDojoConnector> defectDojoConnectorMock, 
        [Frozen]Mock<IMetadataExtractor> metadataExtractorMock, MetadataProcessor sut, 
        AssetProject pi, Metadata metadata, int productId)
    {
        List<(Metadata,bool)> extraction = new () { (metadata,false)};
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(It.IsAny<Metadata>()))
            .ThrowsAsync(new Exception(error));
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().BeEmpty();
        res.Errors.Count.Should().Be(1);
        res.Errors.Should().Contain(e=>e.Message==error);
    }   
    
    [Theory]
    [AutoMoqData]
    public async Task WhenWarning_AddedToRes([Frozen]Mock<IDefectDojoConnector> defectDojoConnectorMock, 
        [Frozen]Mock<IMetadataExtractor> metadataExtractorMock, MetadataProcessor sut, 
        AssetProject pi, Metadata metadata, int productId, WarningAssetProjectProcessor warning)
    {
        List<(Metadata,bool)> extraction = new () { (metadata,false)};
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(It.IsAny<Metadata>()))
            .ThrowsAsync(warning);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().BeEmpty();
        res.Warnings.Count.Should().Be(1);
        res.Warnings.Should().Contain(e=>e.Message==warning.Message);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenNonCriticalErrorOrWarning_ProcessContinue([Frozen]Mock<IDefectDojoConnector> defectDojoConnectorMock, 
        [Frozen]Mock<IMetadataExtractor> metadataExtractorMock, MetadataProcessor sut, 
        AssetProject pi, Metadata metadata1, Metadata metadata2, Metadata metadata3, int productId, WarningAssetProjectProcessor warning, ErrorAssetProjectProcessor error)
    {
        List<(Metadata,bool)> extraction = new () { (metadata1,false),(metadata2,false),(metadata3,false)};
        metadataExtractorMock.Setup(m => m.ExtractMetadata(It.IsAny<AssetProject>(), It.IsAny<int>()))
            .Returns(extraction);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata1))
            .ThrowsAsync(warning);
        defectDojoConnectorMock.Setup(m => m.UpdateMetadataAsync(metadata2))
            .ThrowsAsync(error);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(metadata3))
            .ReturnsAsync(metadata3);
        var res = await sut.ProcessProjectMetadataAsync(pi, ProductAdapterAction.Create, productId);
        res.Entities.Should().Contain(e=>e.DefectDojoId==metadata3.Id);
        res.Warnings.Count.Should().Be(1);
        res.Errors.Count.Should().Be(1);
    }
    
    [Fact]
    public void WhenActionCreateRequired_CreateIsCalled(){
        throw new NotImplementedException();
    }
    
    [Fact]
    public void WhenActionUpdateRequired_UpdateIsCalled(){
        throw new NotImplementedException();
    }
    
    [Fact]
    public void WhenUpdatedRequiredWithoutValidProductId_ErrorInRes(){
        throw new NotImplementedException();
    }
    
        
    [Fact]
    public void WhenInvalidActionRequired_ErrorInRes(){
        throw new NotImplementedException();
    }
}