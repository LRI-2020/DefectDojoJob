using DefectDojoJob.Models;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Models.Processor.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Services;

public class InitialLoadService
{
    private readonly HttpClient httpClient;
    private readonly AssetProjectInfoValidator assetProjectInfoValidator;
    private readonly IConfiguration configuration;

    public InitialLoadService(HttpClient httpClient, AssetProjectInfoValidator assetProjectInfoValidator, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.assetProjectInfoValidator = assetProjectInfoValidator;
        this.configuration = configuration;
    }

    string GetJsonResponseFromFireBase(string data)
    {
        var obj = (IList<JToken>)JObject.Parse(data);
        return ((JProperty)obj[0]).Value.ToString();
    }

    public async Task<InitialLoadResult> FetchInitialLoadAsync()
    {
        if (!DateTimeOffset.TryParse(configuration["LastRunDate"], out DateTimeOffset refDate))
            throw new Exception("Last run date provided is invalid. Please correct the configuration file.");

        var res = new InitialLoadResult();
        var jObjects = await FetchJsonDataAsync();
        
        foreach (var data in jObjects)
        {
            try
            {
                var projectInfo = data.ToObject<AssetProjectInfo>();
                if (projectInfo == null) throw new Exception("Invalid json model provided");
                assetProjectInfoValidator.Validate(projectInfo);
                if(assetProjectInfoValidator.ShouldBeProcessed(refDate,projectInfo)) res.ProjectsToProcess.Add(projectInfo);
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
        var response = await httpClient.GetAsync(configuration["AssetUrl"]);
        var jsonResponse = GetJsonResponseFromFireBase(await response.Content.ReadAsStringAsync());
        return JsonConvert.DeserializeObject<List<JObject>>(jsonResponse) ??
               new List<JObject>();
    }
}