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

    public ProductsProcessor(IDefectDojoConnector defectDojoConnector, IProductExtractor productExtractor)
    {
        this.defectDojoConnector = defectDojoConnector;
        this.productExtractor = productExtractor;
    }

    public async Task<ProductProcessingResult> ProcessProductAsync(AssetProject project,
        List<AssetToDefectDojoMapper> users, ProductAdapterAction action, int? productId = null)
    {
        var res = new ProductProcessingResult();
        try
        {
            var product = await productExtractor.ExtractProduct(project, users);

            switch (action)
            {
                case ProductAdapterAction.Create:
                    res.Entity = await CreateProjectInfoAsync(product, project);
                    break;
                case ProductAdapterAction.Update:
                    product.Id = productId ?? throw new ErrorAssetProjectProcessor(
                        "Update product requested but no productId found or provided", product.Name, EntitiesType.Product);
                    res.Entity = await UpdateProjectInfoAsync(product, project.Code);
                    break;
                case ProductAdapterAction.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
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

    private async Task<AssetToDefectDojoMapper> UpdateProjectInfoAsync(Product product, string code)
    {
        var updateRes = await defectDojoConnector.UpdateProductAsync(product);
        return new AssetToDefectDojoMapper(code, updateRes.Id, EntitiesType.Product);
    }

    private async Task<AssetToDefectDojoMapper> CreateProjectInfoAsync(Product product, AssetProject pi)
    {
        var createRes = await defectDojoConnector.CreateProductAsync(product);
        return new AssetToProductMapper(pi.Code, createRes.Id);
    }
}