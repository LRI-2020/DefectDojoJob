using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Services;

public class InitialLoadRepository
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;

    public InitialLoadRepository(IConfiguration configuration, HttpClient httpClient)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<JObject>> FetchJsonDataAsync()
    {
        var url = !(string.IsNullOrEmpty(configuration["AssetUrl"]))? 
            new Uri(configuration["AssetUrl"]!): throw new Exception($"Invalid url provided'{configuration["AssetUrl"]}'");

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
    
    string GetJsonResponseFromFireBase(string data)
    {
        var obj = (IList<JToken>)JObject.Parse(data);
        return ((JProperty)obj[0]).Value.ToString();
    }
    
}