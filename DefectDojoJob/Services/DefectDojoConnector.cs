using System.Net.Http.Headers;
using System.Text;
using DefectDojoJob.Helpers;
using DefectDojoJob.Models.DefectDojo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        this.httpClient.DefaultRequestHeaders.Add("Access-Control-Expose-Headers", "authorization");

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

            try
            {
                return ((JArray)results)[0].ToObject<DojoGroup>();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
    }

    public async Task CreateDojoGroup(string teamName)
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
//        var res = await httpClient.PostAsync(url, content);
    }
}