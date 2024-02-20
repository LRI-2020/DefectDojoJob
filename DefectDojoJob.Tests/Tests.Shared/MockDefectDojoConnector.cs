using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Services.Interfaces;
using Moq;

namespace DefectDojoJob.Tests.Tests.Shared;

public class MockDefectDojoConnector:Mock<IDefectDojoConnector>
{
    public MockDefectDojoConnector MockGetProductTypeByNameAsync(ProductType productType)
    {
        Setup(mock 
                => mock.GetProductTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productType);
        return this;
    }   
    
    public MockDefectDojoConnector MockGetProductTypeByNameAsync(ProductType productType, string name)
    {
        Setup(mock 
                => mock.GetProductTypeByNameAsync(name))
            .ReturnsAsync(productType);
        return this;
    }

    public MockDefectDojoConnector MockCreateProductAsync(Product output)
    {
        Setup(m 
            => m.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(output);
        return this;
    }   
    
    public MockDefectDojoConnector MockCreateMetadataAsync(Metadata output)
    {
        Setup(m 
            => m.CreateMetadataAsync(It.IsAny<Metadata>())).ReturnsAsync(output);
        return this;
    }

    public MockDefectDojoConnector DefaultSetup(Product product, Metadata metadata, ProductType productType)
    {
        MockCreateMetadataAsync(metadata);
        MockGetProductTypeByNameAsync(productType);
        MockCreateProductAsync(product);
        return this;
    }
}