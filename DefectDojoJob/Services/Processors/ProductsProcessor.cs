using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Models.Processor.Results;
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
                var productId = await ExistingProjectAsync(project);
                switch (productId)
                {
                    case null:
                        result.Entities.Add(await ProcessProductAsync(project, users, AssetProjectInfoProcessingAction.Create));
                        break;
                    case not null:
                        result.Entities.Add(await ProcessProductAsync(project, users, AssetProjectInfoProcessingAction.Update, productId));
                        break;
                }
            }
            
            catch (Exception e)
            {
                if (e is WarningAssetProjectInfoProcessor warning) result.Warnings.Add(warning);
                else
                {
                    result.Errors.Add(new ErrorAssetProjectInfoProcessor(e.Message, project.Code, EntityType.Product));
                }
            }
        }

        return result;
    }

    private async Task<int?> ExistingProjectAsync(AssetProjectInfo project)
    {
        var searchParams = new Dictionary<string, string>
        {
            { "name", "AssetCode" },
            { "value", project.Code }
        };

        var metadataTask = defectDojoConnector.GetMetadataAsync(searchParams);
        var productTask = defectDojoConnector.GetProductByNameAsync(project.Name);

        var metadata = await metadataTask;
        var product = await productTask;
        ValidateResults(metadata, product, project.Code, project.Name);
        if (metadata != null && product != null) return product.Id;
        return null;
    }

    private static void ValidateResults(Metadata? metadata, Product? product, string code, string name)
    {
        const string errorMessage = "Mismatch Code and Name -";
        if (metadata == null && product != null)
            throw new ErrorAssetProjectInfoProcessor($"{errorMessage} Product '{name}' has been found but no code '{code}' linked to it",
                code, EntityType.Product);
        if (metadata != null && product == null)
            throw new ErrorAssetProjectInfoProcessor($"{errorMessage} Code '{code}' has been found but no product '{name}' linked to it",
                code, EntityType.Product);
        if (metadata?.Product != product?.Id)
            throw new ErrorAssetProjectInfoProcessor($"{errorMessage} Code {code} and product '{name}' have been found but are not linked together in defect dojo",
                code, EntityType.Product);
    }

    public async Task<AssetToDefectDojoMapper> ProcessProductAsync(AssetProjectInfo projectInfo,
        List<AssetToDefectDojoMapper> users, AssetProjectInfoProcessingAction requiredAction, int? productId = null)
    {
        var productType = await GetProductTypeAsync(projectInfo.ProductType, projectInfo.Code);

        var product = new Product(projectInfo.Name, SetDescription(projectInfo))
        {
            ProductTypeId = productType,
            Lifecycle = GetLifeCycle(projectInfo.State),
            TechnicalContact = GetUser(projectInfo, nameof(projectInfo.ApplicationOwner), users),
            TeamManager = GetUser(projectInfo, nameof(projectInfo.ApplicationOwnerBackUp), users),
            ProductManager = GetUser(projectInfo, nameof(projectInfo.FunctionalOwner), users),
            UserRecords = projectInfo.NumberOfUsers,
            ExternalAudience = projectInfo.OpenToPartner ?? false
        };

        switch (requiredAction)
        {
            case AssetProjectInfoProcessingAction.Create :
                var createRes = await defectDojoConnector.CreateProductAsync(product);
                return new AssetToDefectDojoMapper(projectInfo.Code, (createRes.Id));
            case AssetProjectInfoProcessingAction.Update:
                product.Id = productId ?? throw new ErrorAssetProjectInfoProcessor("Update product requested but no productId found or provided",product.Name,EntityType.Product);
                var updateRes = await defectDojoConnector.UpdateProductAsync(product);
                return new AssetToDefectDojoMapper(projectInfo.Code, updateRes.Id);
            case AssetProjectInfoProcessingAction.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(requiredAction), requiredAction, "Invalid action required on the product");
        }
    }

    private static string SetDescription(AssetProjectInfo projectInfo)
    {
        return string.IsNullOrEmpty(projectInfo.ShortDescription?.Trim()) && string.IsNullOrEmpty(projectInfo.DetailedDescription?.Trim())
            ? DefaultDescription
            : $"Short Description : {projectInfo.ShortDescription ?? "/"}; Detailed Description : {projectInfo.DetailedDescription ?? "/"} ; ";
    }

    private async Task<int> GetProductTypeAsync(string? providedProductType, string assetIdentifier)
    {
        var defaultType = configuration["ProductType"];

        if (string.IsNullOrEmpty(providedProductType) && string.IsNullOrEmpty(defaultType))
            throw new ErrorAssetProjectInfoProcessor(
                "no product type provided and none found in the configuration file", assetIdentifier, EntityType.Product);

        ProductType? res;
        if (!string.IsNullOrEmpty(providedProductType)) res = await defectDojoConnector.GetProductTypeByNameAsync(providedProductType);
        else res = await defectDojoConnector.GetProductTypeByNameAsync(defaultType!);

        return res?.Id ??
               throw new ErrorAssetProjectInfoProcessor(
                   $"No product type was found, neither with the provided type nor with the default type", 
                   assetIdentifier, EntityType.Product);
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

    private static int? GetUser(AssetProjectInfo pi, string propertyName, List<AssetToDefectDojoMapper> users)
    {
        return users
            .Find(u => u.AssetIdentifier == (string)(pi.GetType().GetProperty(propertyName)?.GetValue(pi) ?? ""))
            ?.DefectDojoId;
    }
}