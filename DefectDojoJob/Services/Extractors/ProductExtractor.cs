using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Extractors;

public class ProductExtractor:IProductExtractor
{
    private readonly IConfiguration configuration;
    private readonly IDefectDojoConnector defectDojoConnector;
    private const string DefaultDescription = "Enter a description";

    public ProductExtractor(IConfiguration configuration, IDefectDojoConnector defectDojoConnector)
    {
        this.configuration = configuration;
        this.defectDojoConnector = defectDojoConnector;
    }

    public async Task<Product> ExtractProduct(AssetProject project, List<AssetToDefectDojoMapper>users)
    {
        var productType = await GetProductTypeAsync(project.ProductType, project.Code);
        return ConstructProductFromProject(project, productType, users);
    }
    

    
    private async Task<int> GetProductTypeAsync(string? providedProductType, string assetIdentifier)
    {
        var defaultType = configuration["ProductType"];

        if (string.IsNullOrEmpty(providedProductType) && string.IsNullOrEmpty(defaultType))
            throw new ErrorAssetProjectProcessor(
                "no product type provided and none found in the configuration file", assetIdentifier, EntitiesType.Product);

        ProductType? res;
        if (!string.IsNullOrEmpty(providedProductType)) res = await defectDojoConnector.GetProductTypeByNameAsync(providedProductType);
        else res = await defectDojoConnector.GetProductTypeByNameAsync(defaultType!);

        return res?.Id ??
               throw new ErrorAssetProjectProcessor(
                   $"No product type was found, neither with the provided type nor with the default type",
                   assetIdentifier, EntitiesType.Product);
    }
    
    private static Product ConstructProductFromProject(AssetProject project, int productType, List<AssetToDefectDojoMapper> users)
    {
        return new Product(project.Name, SetDescription(project))
        {
            ProductTypeId = productType,
            Lifecycle = GetLifeCycle(project.State),
            TechnicalContact = GetUser(project, nameof(project.ApplicationOwner), users),
            TeamManager = GetUser(project, nameof(project.ApplicationOwnerBackUp), users),
            ProductManager = GetUser(project, nameof(project.FunctionalOwner), users),
            UserRecords = project.NumberOfUsers,
            ExternalAudience = project.OpenToPartner ?? false
        };
    }
    
    private static string SetDescription(AssetProject project)
    {
        return string.IsNullOrEmpty(project.ShortDescription?.Trim()) && string.IsNullOrEmpty(project.DetailedDescription?.Trim())
            ? DefaultDescription
            : $"Short Description : {project.ShortDescription ?? "/"}; Detailed Description : {project.DetailedDescription ?? "/"} ; ";
    }
    
    private static Lifecycle? GetLifeCycle(string? state)
    {
        if (string.IsNullOrEmpty(state)) return null;
        switch (state.Trim())
        {
            case "EnConstruction": return Lifecycle.construction;
            case "EnService":
            case "EnCoursDeDeclassement":
                return Lifecycle.production;
            case "Declassee": return Lifecycle.retirement;
            default: return null;
        }
    }

    private static int? GetUser(AssetProject pi, string propertyName, List<AssetToDefectDojoMapper> users)
    {
        return users
            .Find(u => u.AssetIdentifier == (string)(pi.GetType().GetProperty(propertyName)?.GetValue(pi) ?? ""))
            ?.DefectDojoId;
    }
    
}

public interface IProductExtractor
{
    public Task<Product> ExtractProduct(AssetProject project, List<AssetToDefectDojoMapper> users);

}