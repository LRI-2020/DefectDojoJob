using AutoFixture;
using AutoFixture.Xunit2;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;
using DefectDojoJob.Tests.AutoDataAttribute;
using FluentAssertions;
using Moq;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests.ProductsProcessor.Tests;

public class ProcessProductsTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenProcessOk_ProjectsAddedToEntities([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut, ProductType productType, Product productRes, List<AssetProjectInfo> projects,
        List<AssetToDefectDojoMapper> users)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(productRes);

        var res = await sut.ProcessProductsAsync(projects, users);

        res.Entities.Count.Should().Be(projects.Count);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenError_ErrorAddedToResAndProcessingContinue([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut, ProductType productType, Product productRes,
        List<AssetToDefectDojoMapper> users)
    {
        var projects = new Fixture().CreateMany<AssetProjectInfo>(5).ToList();
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(productRes);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.Is<Product>(p => p.Name == projects[2].Name ))).Throws<Exception>();

        var res = await sut.ProcessProductsAsync(projects, users);

        res.Entities.Count.Should().Be(projects.Count-1);
        res.Errors.Count.Should().Be(1);
        
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenProcessInWarning_WarningAddedToResult([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut, ProductType productType, Product productRes,
        List<AssetToDefectDojoMapper> users)
    {
        var projects = new Fixture().CreateMany<AssetProjectInfo>(5).ToList();
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(productRes);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.Is<Product>(p => p.Name == projects[2].Name)))
            .Throws<WarningAssetProjectInfoProcessor>();

        var res = await sut.ProcessProductsAsync(projects, users);

        res.Entities.Count.Should().Be(projects.Count - 1);
        res.Warnings.Count.Should().Be(1);
    }
}