using System.Net.Http.Headers;
using System.Text;
using DefectDojoJob.Helpers;
using DefectDojoJob.Models.DefectDojo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DefectDojoJob.Services;

public class DefectDojoConnector
{
    private readonly HttpClient httpClient;
    private readonly ILogger<DefectDojoConnector> logger;

    public DefectDojoConnector(IConfiguration configuration, HttpClient httpClient, ILogger<DefectDojoConnector> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", configuration["ApiToken"]);

        var url = configuration["DefectDojoBaseUrl"];
        if (url != null) this.httpClient.BaseAddress = new Uri(url);
    }

    public async Task<DojoGroup?> GetDefectDojoGroupByNameAsync(string name)
    {
        var url = QueryStringHelper.BuildUrlWithQueryStringUsingStringConcat("dojo_groups/",
            new Dictionary<string, string>() { { "name", name } });

        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var results = JObject.Parse(await response.Content.ReadAsStringAsync())["results"];
            if (results == null || ((JArray)results).Count == 0) return null;

            return ((JArray)results)[0].ToObject<DojoGroup>();
        }

        throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
    }

    public async Task<int> CreateDojoGroup(string teamName)
    {
        var url = "dojo_groups/";
        var body = new
        {
            name = teamName
        };
        var content = new StringContent(
            JsonConvert.SerializeObject(body),
            Encoding.UTF8,
            "application/json");
        var res = await httpClient.PostAsync(url, content);
        if (!res.IsSuccessStatusCode) throw new Exception("Team could not be created");
        var data = (JObject.Parse(await res.Content.ReadAsStringAsync())["id"])?.ToObject<int>();

        return data ?? throw new Exception($"New team '{teamName}' could not be retrieved");
    }

    public async Task<User?> GetDefectDojoUserByUsername(string applicationOwner)
    {
        var url = QueryStringHelper.BuildUrlWithQueryStringUsingStringConcat(
            "users/",
            new Dictionary<string, string> { { "username", applicationOwner } });
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
        var results = JObject.Parse(await response.Content.ReadAsStringAsync())["results"];
        if (results == null || ((JArray)results).Count == 0) return null;

        return ((JArray)results)[0].ToObject<User>();
    }

    public async Task<int> CreateDojoUser(string username)
    {
        var body = new
        {
            username
        };
        var content = new StringContent(JsonSerializer.Serialize(body),Encoding.UTF8,
            "application/json");
        var response = await httpClient.PostAsync("users/", content);
        if(!response.IsSuccessStatusCode) 
            throw new Exception($"Error while creating the User. Status code : {(int)response.StatusCode} - {response.StatusCode}");
        return (JObject.Parse(await response.Content.ReadAsStringAsync()))["id"]?.ToObject<int>()??
               throw new Exception($"New User '{username}' could not be retrieved");
    }
}