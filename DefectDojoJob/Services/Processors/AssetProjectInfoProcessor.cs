using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

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
        List<AssetToDefectDojoMapper> users = new();
        
        if (extraction.Users.Any())
        {
            processingResult.UsersProcessingResult =
                await usersProcessor.ProcessUsersAsync(extraction.Users.ToList());
            users = processingResult.UsersProcessingResult.Select<UserProcessingResult, AssetToDefectDojoMapper>(r => r.Entity).ToList();
        }

        processingResult.ProductsProcessingResults = await productsProcessor.ProcessProductsAsync(assetProjectInfos, users);

        return processingResult;
    }
}