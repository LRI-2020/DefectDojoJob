using DefectDojoJob.Models.DefectDojo;

namespace DefectDojoJob.Services.Interfaces;

public interface IDefectDojoConnector
{
    public  Task<User?> GetDefectDojoUserByUsername(string applicationOwner);
    public  Task<ProductType?> GetProductTypeByNameAsync(string productTypeName);

    public Task<Product> CreateProductAsync(string projectInfoName, string description, int productType, Lifecycle? lifecycle,
        int? applicationOwnerId, int? applicationOwnerBackUpId, int? functionalOwnerId,
        int? numberOfUsers, bool openToPartner = false);
}