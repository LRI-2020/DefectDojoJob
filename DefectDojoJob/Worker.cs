using System.Net.Http.Json;
using System.Threading.Channels;
using DefectDojoJob.Services;
using Newtonsoft.Json;

namespace DefectDojoJob;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly InitialLoadService initialLoadService;
    private readonly AssetProjectInfoProcessor assetProjectInfoProcessor;

    public Worker(ILogger<Worker> logger, InitialLoadService initialLoadService, AssetProjectInfoProcessor assetProjectInfoProcessor)
    {
        _logger = logger;
        this.initialLoadService = initialLoadService;
        this.assetProjectInfoProcessor = assetProjectInfoProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        var loadResult = (await initialLoadService.FetchInitialLoadAsync());

        var results = await assetProjectInfoProcessor.StartProcessingAsync(loadResult.ProjectsToProcess);

        Console.WriteLine(JsonConvert.SerializeObject(results));
      
    }
}