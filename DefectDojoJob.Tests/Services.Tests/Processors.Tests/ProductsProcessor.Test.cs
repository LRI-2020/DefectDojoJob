using AutoFixture.Xunit2;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;
using DefectDojoJob.Services.Processors;
using DefectDojoJob.Tests.AutoDataAttribute;
using Moq;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests;

public class ProductsProcessorTest
{
    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserExist_ValueIsRetrieved([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, ProductsProcessor sut,
        AssetProjectInfo pi, string username, int userId, ProductType productType, Product res)

    {
        //Arrange
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        pi.ApplicationOwner = username;
        pi.ApplicationOwnerBackUp = username;
        pi.FunctionalOwner = username;

        var users = new List<AssetToDefectDojoMapper>() { new(username, userId) };

        //Act
        await sut.ProcessProduct(pi, users);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.ProductManager == userId && p.TeamManager == userId && p.TechnicalContact == userId)));
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserNotFound_NullIsSent([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, ProductsProcessor sut,
        AssetProjectInfo pi, ProductType productType, Product res)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        var users = new List<AssetToDefectDojoMapper>();
        await sut.ProcessProduct(pi, users);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.ProductManager == null && p.TeamManager == null && p.TechnicalContact == null)));
    }

    [Theory]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(" ")]
    public async Task WhenDescriptionNull_DefaultValueSent(string? desc, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, ProductsProcessor sut,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, ProductType productType, Product res)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        pi.ShortDescription = pi.DetailedDescription = desc;
        await sut.ProcessProduct(pi, users);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.Description == "Enter a description")));
    }
    
    [Theory]
    [InlineAutoMoqData(null,"abcdef")]
    [InlineAutoMoqData("","abcdef")]
    [InlineAutoMoqData(" ","abcdef")]   
    [InlineAutoMoqData("abcdef",null)]
    [InlineAutoMoqData("abcdef","")]
    [InlineAutoMoqData("abcdef","  ")]
    public async Task WhenDescriptionNotNull_ConcatValueSent(string? shortDesc, string? detailedDesc, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, ProductsProcessor sut,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, ProductType productType, Product res)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        pi.ShortDescription = shortDesc;
        pi.DetailedDescription = detailedDesc;
        await sut.ProcessProduct(pi, users);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.Description.Contains(shortDesc??"") && p.Description.Contains(detailedDesc??""))));
    }
    //productType - stop if not found // added the result if found
    //If lifeCycle valid enum - value retrieved; if not set to null
    //Create when all param have value
    //Create with null for opt value
}