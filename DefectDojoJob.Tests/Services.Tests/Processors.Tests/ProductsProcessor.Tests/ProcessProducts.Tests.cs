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
        DefectDojoJob.Services.Processors.ProductsProcessor sut, ProductType productType, Product productRes, Metadata metadataRes, List<AssetProjectInfo> projects,
        List<AssetToDefectDojoMapper> users)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(productRes);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(It.IsAny<Metadata>())).ReturnsAsync(metadataRes);

        var res = await sut.ProcessProductsAsync(projects, users);
       
        //1 product and 1 metadata per project processed;
        var productEntities = res.SelectMany(r => r.Entities)
            .Where(e=>e.EntityType==EntityType.Product).ToList();
        productEntities.Count.Should().Be(projects.Count);
        
        var metadataEntities = res.SelectMany(r => r.Entities)
            .Where(e=>e.EntityType==EntityType.Metadata).ToList();
        metadataEntities.Count.Should().Be(projects.Count);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenError_ErrorAddedToResAndProcessingContinue([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut, ProductType productType, Product productRes, Metadata metadataRes,
        List<AssetToDefectDojoMapper> users)
    {
        var projects = new Fixture().CreateMany<AssetProjectInfo>(5).ToList();
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(It.IsAny<Metadata>())).ReturnsAsync(metadataRes);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(productRes);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.Is<Product>(p => p.Name == projects[2].Name)))
            .Throws<Exception>();

        var res = await sut.ProcessProductsAsync(projects, users);

        var productEntities = res.SelectMany(r => r.Entities)
            .Where(e=>e.EntityType==EntityType.Product).ToList();
        productEntities.Count.Should().Be(projects.Count - 1);
        
        var metadataEntities = res.SelectMany(r => r.Entities)
            .Where(e=>e.EntityType==EntityType.Metadata).ToList();
        metadataEntities.Count.Should().Be(projects.Count - 1);
        
        var errors = res.SelectMany(r => r.Errors).ToList();
        errors.Count.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenProcessInWarning_WarningAddedToResult([Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut, ProductType productType, Product productRes, Metadata metadataRes,
        List<AssetToDefectDojoMapper> users)
    {
        var projects = new Fixture().CreateMany<AssetProjectInfo>(5).ToList();
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);
        defectDojoConnectorMock.Setup(m => m.CreateMetadataAsync(It.IsAny<Metadata>())).ReturnsAsync(metadataRes);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(productRes);
        defectDojoConnectorMock.Setup(m => m.CreateProductAsync(It.Is<Product>(p => p.Name == projects[2].Name)))
            .Throws<WarningAssetProjectInfoProcessor>();

        var res = await sut.ProcessProductsAsync(projects, users);

        var entities = res.SelectMany(r => r.Entities).ToList();
        var warnings = res.SelectMany(r => r.Warnings).ToList();

        entities.Count.Should().Be((projects.Count - 1)*2);
        warnings.Count.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenCodeFoundButNotProject_ErrorAddedToResult(Metadata metadataRes,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProjectInfo pi)
    {
        var projects = new List<AssetProjectInfo> { pi };
        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(metadataRes);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((Product?)null);

        var res = await sut.ProcessProductsAsync(projects, users);

        var entities = res.SelectMany(r => r.Entities).ToList();
        var errors = res.SelectMany(r => r.Errors).ToList();

       entities.Count.Should().Be(0);
        errors.Count.Should().Be(1);
        errors[0].Message.Should().Contain("Mismatch Code and Name");
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNameFoundButNotCode_ErrorAddedToResult(Product productRes,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProjectInfo pi)
    {
        var projects = new List<AssetProjectInfo> { pi };
        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync((Metadata?)null);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(productRes);

        var res = await sut.ProcessProductsAsync(projects, users);

        var entities = res.SelectMany(r => r.Entities).ToList();
        var errors = res.SelectMany(r => r.Errors).ToList();

        entities.Count.Should().Be(0);
        errors.Count.Should().Be(1);
        errors[0].Message.Should().Contain("Mismatch Code and Name");
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenMetadataAndProductIdDifferent_ErrorAddedToResult(Product productRes, Metadata metadataRes,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProjectInfo pi)
    {
        metadataRes.Product = 1;
        productRes.Id = 2;
        var projects = new List<AssetProjectInfo> { pi };
        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(metadataRes);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(productRes);

        var res = await sut.ProcessProductsAsync(projects, users);
        var entities = res.SelectMany(r => r.Entities).ToList();
        var errors = res.SelectMany(r => r.Errors).ToList();

        entities.Count.Should().Be(0);
        errors.Count.Should().Be(1);
        errors[0].Message.Should().Contain("Mismatch Code and Name");
        errors[0].Message.Should().Contain("not linked together");
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenExistingProject_UpdateIsCalled(Product productRes, Metadata metadataRes, ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProjectInfo pi)
    {
        metadataRes.Product = 1;
        productRes.Id = 1;
        var projects = new List<AssetProjectInfo> { pi };
        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(metadataRes);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(productRes);
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);

        await sut.ProcessProductsAsync(projects, users);

        defectDojoConnectorMock.Verify(m => m.UpdateProductAsync(It.IsAny<Product>()), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNotExistingProject_CreateIsCalled(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProjectInfo pi)
    {
        var projects = new List<AssetProjectInfo> { pi };
        defectDojoConnectorMock.Setup(m => m.GetMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync((Metadata?)null);
        defectDojoConnectorMock.Setup(m => m.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((Product?)null);
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);

        await sut.ProcessProductsAsync(projects, users);
        defectDojoConnectorMock.Verify(m => m.CreateProductAsync(It.IsAny<Product>()), Times.Once);
    }


    
}