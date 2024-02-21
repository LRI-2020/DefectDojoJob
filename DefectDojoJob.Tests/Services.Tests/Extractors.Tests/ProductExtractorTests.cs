namespace DefectDojoJob.Tests.Services.Tests.Extractors.Tests;

public class ProductExtractorTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task WhenMandatoryPropertiesOnly_NullForOptionalValues(
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductExtractor sut, ProductType productType, List<AssetToDefectDojoMapper> users)
    {
        //Arrange
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);

        var pi = new Fixture().Build<AssetProject>()
            .Without(pi => pi.ApplicationOwner)
            .Without(pi => pi.FunctionalOwner)
            .Without(pi => pi.ApplicationOwnerBackUp)
            .Without(pi => pi.OpenToPartner)
            .Without(pi => pi.State)
            .Without(pi => pi.NumberOfUsers)
            .Create();
        var res = await sut.ExtractProduct(pi, users);
        res.Should().Match<Product>(p =>
            p.TeamManager == null
            && p.ProductManager == null
            && p.TechnicalContact == null
            && p.UserRecords == null
            && p.ExternalAudience == false
            && p.UserRecords == null
            && p.Lifecycle == null
        );
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task WhenExtract_MinimumValuesCorrectlyMapped([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductExtractor sut, AssetProject pi, List<AssetToDefectDojoMapper> users, ProductType productType)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);

        var res = await sut.ExtractProduct(pi, users);

        res.Should().Match<Product>(p =>
            p.Name == pi.Name &&
            p.UserRecords == pi.NumberOfUsers &&
            p.ExternalAudience == pi.OpenToPartner);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserExist_ValueIsRetrieved([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductExtractor sut, AssetProject pi, string username, int userId, ProductType productType)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);
        //Arrange

        pi.ApplicationOwner = username;
        pi.ApplicationOwnerBackUp = username;
        pi.FunctionalOwner = username;

        var users = new List<AssetToDefectDojoMapper> { new(username, userId, EntitiesType.User) };
        //Act
        var res = await sut.ExtractProduct(pi, users);

        res.Should().Match<Product>(p => 
            p.ProductManager == userId 
            && p.TeamManager == userId 
            && p.TechnicalContact == userId);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserNotFound_NullStored([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductExtractor sut, AssetProject pi, ProductType productType)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);
        var res = await sut.ExtractProduct(pi, new List<AssetToDefectDojoMapper>());

        //Assert
        res.Should().Match<Product>(p => 
            p.ProductManager == null 
            && p.TeamManager == null 
            && p.TechnicalContact == null);
    }

    [Theory]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(" ")]
    public async Task WhenDescriptionNull_DefaultDescriptionSet(string? desc,[Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductExtractor sut, 
        AssetProject pi, List<AssetToDefectDojoMapper> users, ProductType productType)

    {
        //Arrange
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);
        pi.ShortDescription = pi.DetailedDescription = desc;
        var res = await sut.ExtractProduct(pi, users);
        res.Description.Should().Be("Enter a description");
    }

    [Theory]
    [InlineAutoMoqData(null, "abcdef")]
    [InlineAutoMoqData("", "abcdef")]
    [InlineAutoMoqData(" ", "abcdef")]
    [InlineAutoMoqData("abcdef", null)]
    [InlineAutoMoqData("abcdef", "")]
    [InlineAutoMoqData("abcdef", "  ")]
    public async Task WhenDescriptionNotNull_ConcatValueSet(string? shortDesc, string? detailedDesc, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductExtractor sut, 
        AssetProject pi, List<AssetToDefectDojoMapper> users, ProductType productType)

    {
        //Arrange
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);
        pi.ShortDescription = shortDesc;
        pi.DetailedDescription = detailedDesc;
        var res = await sut.ExtractProduct(pi, users);

        res.Should().Match<Product>(p => 
            p.Description.Contains(shortDesc ?? "") 
            && p.Description.Contains(detailedDesc ?? ""));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNoProductTypeFound_ErrorThrown([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        ProductExtractor sut,
        AssetProject pi, List<AssetToDefectDojoMapper> users)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ProductType?)null);

        Func<Task> act = () => sut.ExtractProduct(pi, users);
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.ToLower().Contains("no product type"));
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenNoProductTypeProvided_ErrorThrown(ProductExtractor sut,
        AssetProject pi, List<AssetToDefectDojoMapper> users)

    {
        pi.ProductType = null;
        Func<Task> act = () => sut.ExtractProduct(pi, users);
        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.Message.ToLower().Contains("no product type provided"));
    }

    [Theory]
    [InlineAutoMoqData("EnConstruction", Lifecycle.construction)]
    [InlineAutoMoqData("EnService", Lifecycle.production)]
    [InlineAutoMoqData("EnCoursDeDeclassement", Lifecycle.production)]
    [InlineAutoMoqData("Declassee", Lifecycle.retirement)]
    public async Task WhenStateIsValidLifeCycle_CorrectMatchingValueSent(string? state, Lifecycle expectedLifecycle,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, ProductExtractor sut,
        AssetProject pi, List<AssetToDefectDojoMapper> users, ProductType productType)

    {
        //Arrange

        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);
        pi.State = state;

        //Act
        var res = await sut.ExtractProduct(pi, users);
        //Assert
        res.Lifecycle.Should().Be(expectedLifecycle);
    }

    [Theory]
    [InlineAutoMoqData("unknown")]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("   ")]
    public async Task WhenStateIsInvalidLifeCycle_NullSent(string? state,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, ProductExtractor sut,
        AssetProject pi, List<AssetToDefectDojoMapper> users, ProductType productType)

    {
        //Arrange
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);
        pi.State = state;
        var res =await sut.ExtractProduct(pi, users);

        res.Lifecycle.Should().BeNull();
    }
}