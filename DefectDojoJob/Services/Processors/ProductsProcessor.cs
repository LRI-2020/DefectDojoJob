using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Extractors;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class ProductsProcessor : IProductsProcessor
{
    private readonly IDefectDojoConnector defectDojoConnector;
    private readonly IProductExtractor productExtractor;
    private const string CodeMetadataName = "AssetCode";

    public ProductsProcessor(IDefectDojoConnector defectDojoConnector, IProductExtractor productExtractor)
    {
        this.defectDojoConnector = defectDojoConnector;
        this.productExtractor = productExtractor;
    }

    public async Task<ProductProcessingResult> ProcessProductAsync(AssetProject project,
        List<AssetToDefectDojoMapper> users)
    {
        var res = new ProductProcessingResult();
        try
        {
            var productId = await ExistingProjectAsync(project);
            var product = await productExtractor.ExtractProduct(project, users);

            switch (productId)
            {
                case null:
                    res.Entity = await CreateProjectInfoAsync(product, project);
                    break;
                case not null:
                    product.Id = (int)productId;
                    res.Entity= await UpdateProjectInfoAsync(product, productId, project.Code);
                    break;
            }
        }
        catch (Exception e)
        {
            if (e is WarningAssetProjectProcessor warning) res.Warnings.Add(warning);
            else
            {
                res.Errors.Add(new ErrorAssetProjectProcessor(e.Message, project.Code, EntitiesType.Product));
            }
        }

        return res;

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


    private async Task<AssetToDefectDojoMapper> UpdateProjectInfoAsync(Product product, int? productId, string code)
    {
        if (productId == null)
        {
            throw new ErrorAssetProjectProcessor(
                "Update product requested but no productId found or provided", product.Name, EntitiesType.Product);
        }

        product.Id = (int)productId;
        var updateRes = await defectDojoConnector.UpdateProductAsync(product);
        return new AssetToDefectDojoMapper(code, updateRes.Id, EntitiesType.Product);
    }

    private async Task<AssetToDefectDojoMapper> CreateProjectInfoAsync(Product product, AssetProject pi)
    {
        var createRes = await defectDojoConnector.CreateProductAsync(product);
        return new AssetToProductMapper(pi.Code, createRes.Id);
    }
}