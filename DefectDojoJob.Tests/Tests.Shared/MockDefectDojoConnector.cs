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
    
    public MockDefectDojoConnector MockUpdateProductAsync(Product output)
    {
        Setup(m 
            => m.UpdateProductAsync(It.IsAny<Product>())).ReturnsAsync(output);
        return this;
    }   
    
    public MockDefectDojoConnector MockCreateMetadataAsync(Metadata output)
    {
        Setup(m 
            => m.CreateMetadataAsync(It.IsAny<Metadata>())).ReturnsAsync(output);
        return this;
    }
    public MockDefectDojoConnector MockGetMetadataAsync(Metadata output)
    {
        Setup(m 
            => m.GetMetadataAsync(It.IsAny<Dictionary<string,string>>())).ReturnsAsync(output);
        return this;
        
    }

    public MockDefectDojoConnector DefaultUpdateSetup(Product product, Metadata metadata, ProductType productType)
    {
        MockGetProductByNameAsync(product);
        MockCreateMetadataAsync(metadata);
        MockGetProductTypeByNameAsync(productType);
        MockCreateProductAsync(product);
        MockGetMetadataAsync(metadata);
        MockUpdateProductAsync(product);
        return this;
    }
    
    public MockDefectDojoConnector DefaultCreateSetup(Product product, Metadata metadata, ProductType productType)
    {
        MockCreateMetadataAsync(metadata);
        MockGetProductTypeByNameAsync(productType);
        MockCreateProductAsync(product);
        return this;
    }

    private MockDefectDojoConnector MockGetProductByNameAsync(Product product)
    {
        Setup(m => m.GetProductByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(product);
        return this;
    }
}