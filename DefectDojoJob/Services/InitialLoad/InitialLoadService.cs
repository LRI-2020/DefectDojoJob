using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Services.InitialLoad;

public class InitialLoadService
{
    private readonly HttpClient httpClient;
    private readonly IAssetProjectValidator assetProjectValidator;
    private readonly IConfiguration configuration;

    public InitialLoadService(HttpClient httpClient, IAssetProjectValidator assetProjectValidator, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.assetProjectValidator = assetProjectValidator;
        this.configuration = configuration;
    }

    string GetJsonResponseFromFireBase(string data)
    {
        var obj = (IList<JToken>)JObject.Parse(data);
        return ((JProperty)obj[0]).Value.ToString();
    }

    public async Task<InitialLoadResult> FetchInitialLoadAsync()
    {
        var res = new InitialLoadResult();

        if (!DateTimeOffset.TryParse(configuration["LastRunDate"], out DateTimeOffset refDate))
        {
            res.Errors.Add((null, "Last run date provided is invalid. Please correct the configuration file."));
            return res;
        }

        IEnumerable<JObject> jObjects;

        try
        {
            jObjects = await FetchJsonDataAsync();
        }
        catch (Exception e)
        {
            res.Errors.Add((null, e.Message));
            return res;
        }

        foreach (var data in jObjects)
        {
            try
            {
                var projectInfo = data.ToObject<AssetProject>();
                if (projectInfo == null) throw new Exception("Invalid json model provided");
                assetProjectValidator.Validate(projectInfo);
                if (assetProjectValidator.ShouldBeProcessed(refDate, projectInfo)) res.ProjectsToProcess.Add(projectInfo);
                else res.DiscardedProjects.Add(projectInfo);
            }
            catch (Exception e)
            {
                res.Errors.Add((data, e.Message));
            }
        }

        return res;
    }


    private async Task<IEnumerable<JObject>> FetchJsonDataAsync()
    {
        var url = configuration["AssetUrl"]!;

        try
        {
            var response = await httpClient.GetAsync(url);
            var jsonResponse = GetJsonResponseFromFireBase(await response.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<List<JObject>>(jsonResponse) ??
                   new List<JObject>();
        }

        catch (Exception e)
        {
            throw new Exception($"Error while loading the asset's file at url '{url}': {e.Message}");
        }
    }
}