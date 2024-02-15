using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class ProductsProcessor : IProductsProcessor
{
    private readonly IConfiguration configuration;
    private readonly IDefectDojoConnector defectDojoConnector;
    private const string DefaultDescription = "Enter a description";

    public ProductsProcessor(IConfiguration configuration, IDefectDojoConnector defectDojoConnector)
    {
        this.configuration = configuration;
        this.defectDojoConnector = defectDojoConnector;
    }
    public async Task<ProductsProcessingResult> ProcessProductsAsync(List<AssetProjectInfo> projects,
        List<AssetToDefectDojoMapper> users)
    {
        var result = new ProductsProcessingResult();
        foreach (var project in projects)
        {
            try
            {
                result.Entities.Add(await ProcessProduct(project, users));
            }
            catch (Exception e)
            {
                if (e is WarningAssetProjectInfoProcessor warning) result.Warnings.Add(warning);
                else
                {
                    result.Errors.Add(new ErrorAssetProjectInfoProcessor(e.Message, project.Name, EntityType.Product));
                }
            }
        }

        return result;
    }

    public async Task<AssetToDefectDojoMapper> ProcessProduct(AssetProjectInfo projectInfo,
        List<AssetToDefectDojoMapper> users)
    {
        var description = string.IsNullOrEmpty(projectInfo.ShortDescription?.Trim()) && string.IsNullOrEmpty(projectInfo.DetailedDescription?.Trim())
            ? DefaultDescription
            : $"Short Description : {projectInfo.ShortDescription??"/"}; Detailed Description : {projectInfo.DetailedDescription??"/"} ; ";
        var productType = await GetProductTypeAsync(projectInfo.ProductType, projectInfo.Name);
        var lifecycle = MatchLifeCycle(projectInfo.State);

        var appOwnerId = users
            .Find(u => u.AssetIdentifier == projectInfo.ApplicationOwner)
            ?.DefectDojoId;
        var appOwnerBuId = users
            .Find(u => u.AssetIdentifier == projectInfo.ApplicationOwnerBackUp)?
            .DefectDojoId;
        var funcOwnerId = users
            .Find(u => u.AssetIdentifier == projectInfo.FunctionalOwner)?
            .DefectDojoId;

        var product = new Product(projectInfo.Name, description)
        {
            ProductTypeId = productType,
            Lifecycle = lifecycle,
            TechnicalContact = appOwnerId,
            TeamManager = appOwnerBuId,
            ProductManager = funcOwnerId,
            UserRecords = projectInfo.NumberOfUsers,
            ExternalAudience = projectInfo.OpenToPartner ?? false

        };

        var res = await defectDojoConnector.CreateProductAsync(product);

        return new AssetToDefectDojoMapper(projectInfo.Name, res.Id);
    }

    private async Task<int> GetProductTypeAsync(string? providedProductType, string assetIdentifier)
    {
        var defaultType = configuration["ProductType"];

        if (string.IsNullOrEmpty(providedProductType) && string.IsNullOrEmpty(defaultType))
            throw new ErrorAssetProjectInfoProcessor(
                "no productType provided and none found in the configuration file", assetIdentifier, EntityType.Product);

        ProductType? res;
        if (!string.IsNullOrEmpty(providedProductType)) res = await defectDojoConnector.GetProductTypeByNameAsync(providedProductType);
        else res = await defectDojoConnector.GetProductTypeByNameAsync(defaultType!);

        return res?.Id ??
               throw new ErrorAssetProjectInfoProcessor(
                   $"No product type was found, neither with the provided type nor with the default type", assetIdentifier, EntityType.Product);
    }

    private static Lifecycle? MatchLifeCycle(string? state)
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
}