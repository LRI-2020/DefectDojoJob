using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services;

public class AssetProjectInfoProcessor
{
    private readonly IEntitiesExtractor entitiesExtractor;
    private readonly IUsersProcessor usersProcessor;
    private readonly IProductsProcessor productsProcessor;

    public AssetProjectInfoProcessor( 
        IEntitiesExtractor entitiesExtractor,
        IUsersProcessor usersProcessor,
        IProductsProcessor productsProcessor)
    {
        this.entitiesExtractor = entitiesExtractor;
        this.usersProcessor = usersProcessor;
        this.productsProcessor = productsProcessor;
    }

    public async Task<ProcessingResult> StartProcessingAsync(List<AssetProjectInfo> assetProjectInfos)
    {
        var processingResult = new ProcessingResult();

        if (!assetProjectInfos.Any()) return processingResult;

        var extraction = entitiesExtractor.ExtractEntities(assetProjectInfos);
        if (extraction.Users.Any()) processingResult.UsersProcessingResult = await usersProcessor.ProcessUsersAsync(extraction.Users.ToList());

        processingResult.ProductsProcessingResult = await productsProcessor.ProcessProductsAsync(assetProjectInfos,
            processingResult.UsersProcessingResult.Entities);

        return processingResult;
    }



}