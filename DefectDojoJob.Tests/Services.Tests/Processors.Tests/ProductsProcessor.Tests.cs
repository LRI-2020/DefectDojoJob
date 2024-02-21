using AutoFixture;
using AutoFixture.Xunit2;
using DefectDojoJob.Models.Adapters;
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

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests;

public class ProductsProcessorTests
{


    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateCallWithoutProductId_Error(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);

        Func<Task> act = () => sut.ProcessProductAsync(pi, users, ProductAdapterAction.Update);
        await act.Should().ThrowAsync<ErrorAssetProjectProcessor>()
            .Where(e => e.Message.Contains("no productId"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenInvalidAction_Error(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    {
        defectDojoConnectorMock.Setup(m => m.GetProductTypeByNameAsync(It.IsAny<string>())).ReturnsAsync(productType);

        Func<Task> act = () => sut.ProcessProductAsync(pi, users, ProductAdapterAction.None);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .Where(e => e.Message.Contains("Invalid action"));
    }

    
    [Theory]
    [AutoMoqData]
    public async Task WhenError_AddedToRes(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    
    
    [Theory]
    [AutoMoqData]
    public async Task WhenWarning_AddedToResult(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)

    [Theory]
    [AutoMoqData]
    public async Task WhenCreateREquired_CreateCalled(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateRequired_UpdateCalled(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)

    [Theory]
    [AutoMoqData]
    public async Task WhenCreateOk_AssetMapperAddedToRes(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
    
    [Theory]
    [AutoMoqData]
    public async Task WhenUpdateOk_AssetMapperAddedToRes(ProductType productType,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        DefectDojoJob.Services.Processors.ProductsProcessor sut,
        List<AssetToDefectDojoMapper> users, AssetProject pi)
}