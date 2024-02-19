using AutoFixture;
using AutoFixture.Xunit2;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services;
using DefectDojoJob.Services.Interfaces;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Helpers.Tests;
using FluentAssertions;
using Moq;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests.ProductsProcessor.Tests;

public class ProcessProductTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task WhenCreateWithMandatoryPropertiesOnly_NullSentForOptionalValues([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        ProductType productType, Product res)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);
        
        var pi = (new Fixture()).Build<AssetProjectInfo>()
            .Without(pi => pi.ApplicationOwner)
            .Without(pi=> pi.FunctionalOwner)
            .Without(pi=> pi.ApplicationOwnerBackUp)
            .Without(pi=> pi.OpenToPartner)
            .Without(pi=> pi.State)
            .Without(pi=> pi.NumberOfUsers)
            .Create();
        var users = new List<AssetToDefectDojoMapper>();
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p =>
                p.TeamManager == null
                && p.ProductManager == null
                && p.TechnicalContact == null
                && p.UserRecords == null
                && p.ExternalAudience == false
                && p.UserRecords == null 
                && p.Lifecycle == null)));
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task WhenCreate_DirectValuesCorrectlyMapped([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        AssetProjectInfo pi, ProductType productType, Product res)
    {
         defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        var users = new List<AssetToDefectDojoMapper>();
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p =>
                p.Name == pi.Name &&
                p.UserRecords == pi.NumberOfUsers &&
                p.ExternalAudience == pi.OpenToPartner)));
    }


    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserExist_ValueIsRetrieved([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
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
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.ProductManager == userId && p.TeamManager == userId && p.TechnicalContact == userId)));
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserNotFound_NullIsSent([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        AssetProjectInfo pi, ProductType productType, Product res)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        var users = new List<AssetToDefectDojoMapper>();
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.ProductManager == null && p.TeamManager == null && p.TechnicalContact == null)));
    }

    [Theory]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(" ")]
    public async Task WhenDescriptionNull_DefaultValueSent(string? desc, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, ProductType productType, Product res)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        pi.ShortDescription = pi.DetailedDescription = desc;
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.Description == "Enter a description")));
    }

    [Theory]
    [InlineAutoMoqData(null, "abcdef")]
    [InlineAutoMoqData("", "abcdef")]
    [InlineAutoMoqData(" ", "abcdef")]
    [InlineAutoMoqData("abcdef", null)]
    [InlineAutoMoqData("abcdef", "")]
    [InlineAutoMoqData("abcdef", "  ")]
    public async Task WhenDescriptionNotNull_ConcatValueSent(string? shortDesc, string? detailedDesc, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, ProductType productType, Product res)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        pi.ShortDescription = shortDesc;
        pi.DetailedDescription = detailedDesc;
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.Description.Contains(shortDesc ?? "") && p.Description.Contains(detailedDesc ?? ""))));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNoProductTypeFound_Error([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ProductType?)null);

        Func<Task> act = () => sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);
        await act.Should().ThrowAsync<Exception>().Where(e => e.Message.ToLower().Contains("no product type"));
    }

    [Theory]
    [InlineAutoMoqData("EnConstruction", Lifecycle.construction)]
    [InlineAutoMoqData("EnService", Lifecycle.production)]
    [InlineAutoMoqData("EnCoursDeDeclassement", Lifecycle.production)]
    [InlineAutoMoqData("Declassee", Lifecycle.retirement)]
    public async Task WhenStateIsValidLifeCycle_CorrectMatchingValueSent(string? state, Lifecycle expectedLifecycle, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, Product res, ProductType type)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(type);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        pi.State = state;
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(It.Is<Product>(p => p.Lifecycle == expectedLifecycle)));
    }

    [Theory]
    [InlineAutoMoqData("unknown")]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("   ")]
    public async Task WhenStateIsInvalidLifeCycle_NullSent(string? state, [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock, DefectDojoJob.Services.Processors.ProductsProcessor sut,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, Product res, ProductType type)

    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(type);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(res);

        pi.State = state;
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(It.Is<Product>(p => p.Lifecycle == null)));
    }
  
}