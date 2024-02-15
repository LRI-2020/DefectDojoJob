using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IDefectDojoConnector
{
    public  Task<User?> GetDefectDojoUserByUsername(string applicationOwner);
    public  Task<ProductType?> GetProductTypeByNameAsync(string productTypeName);

    public Task<Product> CreateProductAsync(Product product);
}