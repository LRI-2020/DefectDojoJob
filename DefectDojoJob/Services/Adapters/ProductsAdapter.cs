using DefectDojoJob.Models.Extractions;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Adapters;

public class ProductsAdapter
{
    private readonly IProductRelatedEntitiesConverter productRelatedEntitiesConverter;
    private readonly IProductsProcessor productsProcessor;

    public ProductsAdapter(IProductRelatedEntitiesConverter productRelatedEntitiesConverter, IProductsProcessor productsProcessor)
    {
        this.productRelatedEntitiesConverter = productRelatedEntitiesConverter;
        this.productsProcessor = productsProcessor;
    }

    public void StartAdapterAsync(List<AssetProject> projects, List<AssetToDefectDojoMapper> users)
    {
        foreach (var project in projects)
        {
            var entitiesExtraction = productRelatedEntitiesConverter.ConvertProductRelatedEntities(project);
            AdaptProject(entitiesExtraction, users);
        }
    }

    public void AdaptProject(ProductAdapterResult conversion, List<AssetToDefectDojoMapper> users)
    {
        //var productResult = await productsProcessor.ProcessProductAsync(conversion.Product, users);
    }
    
    //Start adapter
    //Foreach project, extract entities
    //Then process in creation or update
}