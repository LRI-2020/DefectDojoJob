using DefectDojoJob;
using DefectDojoJob.Services.Adapters;
using DefectDojoJob.Services.DefectDojoConnectors;
using DefectDojoJob.Services.Extractors;
using DefectDojoJob.Services.InitialLoad;
using DefectDojoJob.Services.Interfaces;
using DefectDojoJob.Services.Processors;

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
        services.AddHttpClient<IDefectDojoConnector,DefectDojoConnector>();
        services.AddTransient<InitialLoadService>();
        services.AddTransient<IUsersAdapter,UsersAdapter>();
        services.AddTransient<IUsersExtractor,UsersExtractor>();
        services.AddTransient<IUsersProcessor,UsersProcessor>();
        services.AddTransient<IGroupsProcessor,GroupsProcessor>();
        services.AddTransient<IProjectsAdapter,ProjectsAdapter>();
        services.AddTransient<IProductExtractor,ProductExtractor>();
        services.AddTransient<IProductsProcessor,ProductsProcessor>();
        services.AddTransient<IMetadataExtractor,MetadataExtractor>();
        services.AddTransient<IMetadataProcessor,MetadataProcessor>();
        services.AddTransient<AssetProjectsProcessor>();
        services.AddSingleton<IAssetProjectValidator,AssetProjectValidator>();
        services.AddLogging(o => { o.AddConsole(); });
    })
    .Build();

await host.RunAsync();