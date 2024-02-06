using System.Net.Http.Json;
using System.Text.Json;
using DefectDojoJob.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

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

    public async Task<IEnumerable<AssetProjectInfo>> FetchInitialLoadAsync()
    {
        var response = await httpClient.GetAsync(configuration["AssetUrl"]);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        //chipotage à cause de firebase - endpoint mock
        var obj = (IList<JToken>)JObject.Parse(jsonResponse);
        var allData = ((JProperty)obj[0]).Value;
        
        var invalid = new List<JObject>(); //TODO do something with invalids
        var valid = new List<AssetProjectInfo>();
        List<(JToken token, IList<string> errors)> errorMessages = new ();
        
        foreach (var jToken in allData)
        {
            var data = jToken.ToObject<JObject>();
            IList<string> messages = new List<string>();
            if (data != null && !data.IsValid(assetProjectInfoValidator.GetValidationSchema(),out messages))
            {
                invalid.Add(data);
                errorMessages.Add((jToken,messages));
                continue;
            }

            var asset = jToken.ToObject<AssetProjectInfo>();
            if (asset != null && assetProjectInfoValidator.HasRequiredProperties(asset)) valid.Add(asset);

            else
            {
               if(data != null) invalid.Add(data);
            }
        }
        return valid;
    }

}