using System.Net.Http.Json;
using System.Text.Json;
using DefectDojoJob.Models;

namespace DefectDojoJob.Services;

public class AssetProjectInfoProcessor
{
    public IConfiguration Configuration { get; }

    public AssetProjectInfoProcessor(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public bool TeamProcessor()
    {
        return false;

    }
    
    
    
}