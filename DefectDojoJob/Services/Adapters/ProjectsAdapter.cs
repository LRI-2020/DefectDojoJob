using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;
using DefectDojoJob.Services.Processors;

namespace DefectDojoJob.Services.Adapters;

public class ProjectsAdapter : IProjectsAdapter
{
    private readonly IProductsProcessor productsProcessor;
    private readonly IDefectDojoConnector defectDojoConnector;
    private readonly IMetadataProcessor metadataProcessor;
    private const string CodeMetadataName = "AssetCode";
    public ProjectsAdapter( IProductsProcessor productsProcessor, IDefectDojoConnector defectDojoConnector, IMetadataProcessor metadataProcessor)
    {
        this.productsProcessor = productsProcessor;
        this.defectDojoConnector = defectDojoConnector;
        this.metadataProcessor = metadataProcessor;
    }

    public async Task<List<ProductAdapterResult>> StartAdapterAsync(List<AssetProject> projects, List<AssetToDefectDojoMapper> users)
    {
        var result = new List<ProductAdapterResult>();
        foreach (var project in projects)
        {
            var res = new ProductAdapterResult();
            try
            {
                res = await AdaptProjectAsync(project, users);
            }
            catch (Exception e)
            {
                if (e is WarningAssetProjectProcessor)
                    res.Warnings.Add(new WarningAssetProjectProcessor("", project.Code));
                else
                    res.Errors.Add(new ErrorAssetProjectProcessor("", project.Code));
            }
            result.Add(res);
        }

        return result;
    }

    public async Task<ProductAdapterResult> AdaptProjectAsync(AssetProject project, List<AssetToDefectDojoMapper> users)
    {
        var result = new ProductAdapterResult();
        var productId = await ExistingProjectAsync(project);
        var actionNeeded = productId != null ? ProductAdapterAction.Update : ProductAdapterAction.Create;
        result.ProductResult = await productsProcessor.ProcessProductAsync(project, users, actionNeeded, productId);
        var product = result.ProductResult.Entity;
        if(product != null)
            result.MetadataResults = await metadataProcessor.ProcessProjectMetadataAsync(project, actionNeeded, product.DefectDojoId);
        //Engagements
        //Endpoints
        //ProductGroup
        
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
    
}