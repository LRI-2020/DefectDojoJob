using System.ComponentModel.DataAnnotations;
using DefectDojoJob.Models.Adapters;
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
    private const string CodeMetadataName = "AssetCode";

    public ProductsProcessor(IConfiguration configuration, IDefectDojoConnector defectDojoConnector)
    {
        this.configuration = configuration;
        this.defectDojoConnector = defectDojoConnector;
    }

    public async Task<List<ProductProcessingResult>> ProcessProductsAsync(List<AssetProject> projects,
        List<AssetToDefectDojoMapper> users)
    {
        var result = new List<ProductProcessingResult>();
        foreach (var project in projects)
        {
            var res = new ProductProcessingResult();
            try
            {
                var productId = await ExistingProjectAsync(project);
                res = productId switch
                {
                    null => await ProcessProductAsync(project, users, ProductAdapterAction.Create),
                    not null => await ProcessProductAsync(project, users, ProductAdapterAction.Update, productId)
                };
            }
            catch (Exception e)
            {
                if (e is WarningAssetProjectProcessor warning) res.Warnings.Add(warning);
                else
                {
                    res.Errors.Add(new ErrorAssetProjectProcessor(e.Message, project.Code, EntitiesType.Product));
                }
            }

            result.Add(res);
        }

        return result;
    }

    private async Task<int?> ExistingProjectAsync(AssetProject project)
    {
        var searchParams = new Dictionary<string, string>
        {
            { "name", CodeMetadataName },
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
            throw new ErrorAssetProjectProcessor($"{errorMessage} Product '{name}' has been found but no code '{code}' linked to it",
                code, EntitiesType.Product);
        if (metadata != null && product == null)
            throw new ErrorAssetProjectProcessor($"{errorMessage} Code '{code}' has been found but no product '{name}' linked to it",
                code, EntitiesType.Product);
        if (metadata?.Product != product?.Id)
            throw new ErrorAssetProjectProcessor($"{errorMessage} Code {code} and product '{name}' have been found but are not linked together in defect dojo",
                code, EntitiesType.Product);
    }

    public async Task<ProductProcessingResult> ProcessProductAsync(AssetProject project,
        List<AssetToDefectDojoMapper> users, ProductAdapterAction requiredAction, int? productId = null)
    {
        ProductProcessingResult result;
        var productType = await GetProductTypeAsync(project.ProductType, project.Code);
        var product = ConstructProductFromProject(project, productType, users);

        switch (requiredAction)
        {
            case ProductAdapterAction.Create:
                result = await CreateProjectInfoAsync(product, project);
                break;
            case ProductAdapterAction.Update:
                result = await UpdateProjectInfoAsync(product, productId, project.Code);
                break;
            case ProductAdapterAction.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(requiredAction), requiredAction, "Invalid action required on the product");
        }

        return result;
    }

    private async Task<ProductProcessingResult> UpdateProjectInfoAsync(Product product, int? productId, string code)
    {
        var res = new ProductProcessingResult();
        if (productId == null)
        {
            throw new ErrorAssetProjectProcessor(
                "Update product requested but no productId found or provided", product.Name, EntitiesType.Product);
        }
            
        product.Id = (int)productId;
        var updateRes = await defectDojoConnector.UpdateProductAsync(product);
        res.Entity = new AssetToDefectDojoMapper(code, updateRes.Id, EntitiesType.Product);
        return res;
    }

    private async Task<ProductProcessingResult> CreateProjectInfoAsync(Product product, AssetProject pi)
    {
        var res = new ProductProcessingResult();
        var createRes = await defectDojoConnector.CreateProductAsync(product);
        res.Entity = new AssetToProductMapper(pi.Code, createRes.Id);

        try
        {
            var metadataRes = await defectDojoConnector.CreateMetadataAsync
                (ConstructMetadata(CodeMetadataName, pi.Code, product.ProductTypeId));
            res.MetadataMappers.Add(new AssetToMetadataMapper(metadataRes.Value, createRes.Id));

        }
        catch (Exception)
        {
            if (await defectDojoConnector.DeleteProductAsync(createRes.Id))
            {
                res.Errors.Add(new ErrorAssetProjectProcessor($"Metadata with AssetCode could not be created; Compensation successful- Product with Id '{createRes.Id}' with code {pi.Code} has been deleted",
                    pi.Code,EntitiesType.Product));
            }
            res.Errors.Add(new ErrorAssetProjectProcessor($"Metadata with AssetCode could not be created; Compensation has failed - Product with Id '{createRes.Id}' with code {pi.Code} could not be deleted.PLease clean DefectDojo manually",
                pi.Code,EntitiesType.Product));
        }

        return res;
    }

    private static Metadata ConstructMetadata(string name, string value, int productId)
    {
        return new Metadata
        {
            Name = name,
            Product = productId,
            Value = value
        };
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