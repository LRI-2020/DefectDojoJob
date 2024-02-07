using DefectDojoJob.Services;

namespace DefectDojoJob;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly InitialLoadService initialLoadService;
    private readonly DefectDojoConnector defectDojoConnector;

    public Worker(ILogger<Worker> logger, InitialLoadService initialLoadService, DefectDojoConnector defectDojoConnector)
    {
        _logger = logger;
        this.initialLoadService = initialLoadService;
        this.defectDojoConnector = defectDojoConnector;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var projectsInfo = await initialLoadService.FetchInitialLoadAsync();
           var res = await defectDojoConnector.GetDefectDojoGroupByName();
    }
}