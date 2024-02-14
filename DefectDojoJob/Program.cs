using System.Net.Http.Headers;
using DefectDojoJob;
using DefectDojoJob.Models.Processor.Interfaces;
using DefectDojoJob.Services;
using DefectDojoJob.Services.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .Build();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHttpClient<DefectDojoConnector>();
        services.AddTransient<InitialLoadService>();
        services.AddTransient<IEntitiesExtractor,EntitiesExtractor>();
        services.AddTransient<IProductsProcessor,ProductsProcessor>();
        services.AddTransient<IUsersProcessor,UsersProcessor>();
        services.AddTransient<AssetProjectInfoProcessor>();
        services.AddSingleton<IAssetProjectInfoValidator,AssetProjectInfoValidator>();
        services.AddLogging(o => { o.AddConsole(); });
    })
    .Build();

await host.RunAsync();