using DefectDojoJob;
using DefectDojoJob.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<HttpClient>();
        services.AddTransient<InitialLoadService>();
        services.AddSingleton<AssetProjectInfoValidator>();
    })
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .Build();
    } )
    .Build();

await host.RunAsync();