using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IDefectDojoConnector
{
    public  Task<User?> GetDefectDojoUserByUsernameAsync(string applicationOwner);
    public  Task<ProductType?> GetProductTypeByNameAsync(string productTypeName);
    public Task<Product> CreateProductAsync(Product product);

    public Task<Metadata?> GetMetadataAsync(Dictionary<string, string> searchParams);
    public Task<Product?> GetProductByNameAsync(string name);
    Task<Product> UpdateProductAsync(Product product);
    Task<Metadata> CreateMetadataAsync(Metadata metadata);
    Task<bool> DeleteProductAsync(int productId);
    Task<Metadata> UpdateMetadataAsync(Metadata metadata);
    Task<bool> DeleteMetadataAsync(int metadataId);
}