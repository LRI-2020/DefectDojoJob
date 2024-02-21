using System.Net.Http.Json;
using System.Threading.Channels;
using DefectDojoJob.Services;
using DefectDojoJob.Services.InitialLoad;
using DefectDojoJob.Services.Processors;
using Newtonsoft.Json;

namespace DefectDojoJob;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly InitialLoadService initialLoadService;
    private readonly AssetProjectsProcessor assetProjectsProcessor;

    public Worker(ILogger<Worker> logger, InitialLoadService initialLoadService, AssetProjectsProcessor assetProjectsProcessor)
    {
        _logger = logger;
        this.initialLoadService = initialLoadService;
        this.assetProjectsProcessor = assetProjectsProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        var loadResult = (await initialLoadService.FetchInitialLoadAsync());

        var results = await assetProjectsProcessor.StartProcessingAsync(loadResult.ProjectsToProcess);

        Console.WriteLine(JsonConvert.SerializeObject(results));
      
    }
}