using System.Net.Http.Headers;
using DefectDojoJob;
using DefectDojoJob.Services;

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
        services.AddSingleton<AssetProjectInfoValidator>();
    })

    .Build();

await host.RunAsync();