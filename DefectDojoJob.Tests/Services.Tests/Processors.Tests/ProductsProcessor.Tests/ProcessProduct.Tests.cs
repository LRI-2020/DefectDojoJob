using AutoFixture;
using AutoFixture.Xunit2;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services;
using DefectDojoJob.Services.Interfaces;
using DefectDojoJob.Tests.AutoDataAttribute;
using DefectDojoJob.Tests.Tests.Shared;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests.ProductsProcessor.Tests;

public class ProcessProductTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task WhenCreateWithMandatoryPropertiesOnly_NullSentForOptionalValues(IConfiguration configuration, 
        Metadata metadata, ProductType productType, Product product, List<AssetToDefectDojoMapper> users)
    {
        //Arrange
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);
        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);

        var pi = (new Fixture()).Build<AssetProjectInfo>()
            .Without(pi => pi.ApplicationOwner)
            .Without(pi => pi.FunctionalOwner)
            .Without(pi => pi.ApplicationOwnerBackUp)
            .Without(pi => pi.OpenToPartner)
            .Without(pi => pi.State)
            .Without(pi => pi.NumberOfUsers)
            .Create();
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
    public async Task WhenCreate_DirectValuesCorrectlyMapped(IConfiguration configuration,
        AssetProjectInfo pi, ProductType productType, Product product, Metadata metadata,List<AssetToDefectDojoMapper> users)
    {
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);

        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p =>
                p.Name == pi.Name &&
                p.UserRecords == pi.NumberOfUsers &&
                p.ExternalAudience == pi.OpenToPartner)));
    }


    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserExist_ValueIsRetrieved(IConfiguration configuration,
        AssetProjectInfo pi, string username, int userId, ProductType productType, Product product, Metadata metadata)

    {
        //Arrange
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);

        pi.ApplicationOwner = username;
        pi.ApplicationOwnerBackUp = username;
        pi.FunctionalOwner = username;

        var users = new List<AssetToDefectDojoMapper> { new(username, userId, EntityType.User) };
        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);
        //Act
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.ProductManager == userId && p.TeamManager == userId && p.TechnicalContact == userId)));
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task WhenUserNotFound_NullIsSent(IConfiguration configuration, Metadata metadata, Product product,
        AssetProjectInfo pi, ProductType productType,List<AssetToDefectDojoMapper> users)

    {
        //Arrange
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);
        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);
        
        //Act
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        //Assert
        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(
            It.Is<Product>(p => p.ProductManager == null && p.TeamManager == null && p.TechnicalContact == null)));
    }

    [Theory]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(" ")]
    public async Task WhenDescriptionNull_DefaultValueSent(string? desc, IConfiguration configuration,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, ProductType productType, Product product, Metadata metadata)

    {
        //Arrange
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);
        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);

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
    public async Task WhenDescriptionNotNull_ConcatValueSent(string? shortDesc, string? detailedDesc,IConfiguration configuration,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, ProductType productType, Product product,Metadata metadata)

    {
        //Arrange
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);
        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);

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
    public async Task WhenStateIsValidLifeCycle_CorrectMatchingValueSent(string? state, Lifecycle expectedLifecycle, IConfiguration configuration,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, Product product, ProductType productType, Metadata metadata)

    {
        //Arrange
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);
        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);

        pi.State = state;
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(It.Is<Product>(p => p.Lifecycle == expectedLifecycle)));
    }

    [Theory]
    [InlineAutoMoqData("unknown")]
    [InlineAutoMoqData(null)]
    [InlineAutoMoqData("   ")]
    public async Task WhenStateIsInvalidLifeCycle_NullSent(string? state, 
      IConfiguration configuration, Metadata metadata,
        AssetProjectInfo pi, List<AssetToDefectDojoMapper> users, Product product, ProductType productType)

    {
        //Arrange
        var defectDojoConnectorMock = new MockDefectDojoConnector();
        defectDojoConnectorMock.DefaultSetup(product, metadata, productType);
        var sut = new DefectDojoJob.Services.Processors.ProductsProcessor(configuration, defectDojoConnectorMock.Object);

        pi.State = state;
        await sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Create);

        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(It.Is<Product>(p => p.Lifecycle == null)));
    }


    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateCallWithoutProductId_Error(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProjectInfo pi)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);

        Func<Task> act = () => sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.Update);
        await act.Should().ThrowAsync<ErrorAssetProjectInfoProcessor>()
            .Where(e => e.Message.Contains("no productId"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateCallWithInvalidAction_Error(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProjectInfo pi)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);

        Func<Task> act = () => sut.ProcessProductAsync(pi, users, AssetProjectInfoProcessingAction.None);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .Where(e => e.Message.Contains("Invalid action"));
    }
}