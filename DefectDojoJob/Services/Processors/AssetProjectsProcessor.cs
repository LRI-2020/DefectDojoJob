using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Adapters;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class AssetProjectsProcessor
{
    private readonly IUsersAdapter usersAdapter;
    private readonly IProjectsAdapter projectsAdapter;

    public AssetProjectsProcessor(
        IUsersAdapter usersAdapter,
        IProjectsAdapter projectsAdapter
        )
    {
        this.usersAdapter = usersAdapter;
        this.projectsAdapter = projectsAdapter;
    }

    public async Task<ProcessingResult> StartProcessingAsync(List<AssetProject> assetProjects)
    {
        var processingResult = new ProcessingResult();

        if (!assetProjects.Any())
        {
            processingResult.GeneralErrors.Add("No project to process. Please verify input file");
            return processingResult;
        }

        //Users Adapter
        var usersAdapterRes = await usersAdapter.StartUsersAdapterAsync(assetProjects);
        processingResult.UsersProcessingResult = usersAdapterRes.UsersProcessingResult;
        processingResult.DojoGroupsProcessingResult = usersAdapterRes.DojoGroupsProcessingResult;

        //ProductsAdapter
        var users = usersAdapterRes.UsersProcessingResult.Entities;
        processingResult.ProductsAdapterResults = await projectsAdapter.StartAdapterAsync(assetProjects, users);
        
        return processingResult;
    }
}