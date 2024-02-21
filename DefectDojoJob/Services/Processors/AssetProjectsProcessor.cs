using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class AssetProjectsProcessor
{
    private readonly IUsersAdapter usersAdapter;
    private readonly IProductsProcessor productsProcessor;

    public AssetProjectsProcessor(
        IUsersAdapter usersAdapter,
        IProductsProcessor productsProcessor)
    {
        this.usersAdapter = usersAdapter;
        this.productsProcessor = productsProcessor;
    }

    public async Task<ProcessingResult> StartProcessingAsync(List<AssetProject> assetProjectInfos)
    {
        var processingResult = new ProcessingResult();

        if (!assetProjectInfos.Any())
        {
            processingResult.GeneralErrors.Add("No project to process. Please verify input file");
            return processingResult;
        }

        //Users Adapter
        var usersAdapterRes = await usersAdapter.StartUsersAdapterAsync(assetProjectInfos);
        processingResult.UsersProcessingResult = usersAdapterRes.UsersProcessingResult;
        processingResult.DojoGroupsProcessingResult = usersAdapterRes.DojoGroupsProcessingResult;

        //ProductsAdapter
        var users = usersAdapterRes.UsersProcessingResult.Entities;
        processingResult.ProductsProcessingResults =
            await productsProcessor.ProcessProductsAsync(assetProjectInfos, users);

        return processingResult;
    }
}