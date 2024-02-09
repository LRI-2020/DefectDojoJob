using DefectDojoJob.Models;
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

    public async Task<IEnumerable<AssetProjectInfo>> FetchInitialLoadAsync()
    {
        var jObjects = await FetchJsonDataAsync();
        List<(JObject jObject, string error)> errors = new();

        var res = new List<AssetProjectInfo>();
        foreach (var data in jObjects)
        {
            try
            {
                var projectInfo = data.ToObject<AssetProjectInfo>();
                if (projectInfo == null) throw new Exception("Invalid json model provided");
                assetProjectInfoValidator.Validate(projectInfo);
                res.Add(projectInfo);
            }
            catch (Exception e)
            {
                errors.Add((data, e.Message));
                //TODO do something with errors
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