using DefectDojoJob.Services;

namespace DefectDojoJob;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly InitialLoadService initialLoadService;

    public Worker(ILogger<Worker> logger, InitialLoadService initialLoadService)
    {
        _logger = logger;
        this.initialLoadService = initialLoadService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await initialLoadService.FetchInitialLoadAsync();
        
    }
}