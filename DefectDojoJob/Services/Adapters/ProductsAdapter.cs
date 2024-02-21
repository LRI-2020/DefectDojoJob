using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Adapters;

public class ProjectsAdapter : IProjectsAdapter
{
    private readonly IProductsProcessor productsProcessor;

    public ProjectsAdapter( IProductsProcessor productsProcessor)
    {
        this.productsProcessor = productsProcessor;
    }

    public async Task<List<ProductAdapterResult>> StartAdapterAsync(List<AssetProject> projects, List<AssetToDefectDojoMapper> users)
    {
        var res = new List<ProductAdapterResult>();
        foreach (var project in projects)
        {
            res.Add(await AdaptProjectAsync(project, users));
        }

        return res;
    }

    public async Task<ProductAdapterResult> AdaptProjectAsync(AssetProject project, List<AssetToDefectDojoMapper> users)
    {
        var result = new ProductAdapterResult();
        result.ProductResult = await productsProcessor.ProcessProductAsync(project, users);

        return result;
    }
    
    //Foreach project, extract entities
    //Then process in creation or update
}

public interface IProjectsAdapter
{
    public Task<List<ProductAdapterResult>> StartAdapterAsync(List<AssetProject> projects, List<AssetToDefectDojoMapper> users);
}