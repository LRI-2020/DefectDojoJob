using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using DefectDojoJob.Helpers;
using DefectDojoJob.Models.DefectDojo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DefectDojoJob.Services;

public class DefectDojoConnector
{
    private readonly HttpClient httpClient;

    public DefectDojoConnector(IConfiguration configuration, HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", configuration["ApiToken"]);

        var url = configuration["DefectDojoBaseUrl"];
        if (url != null) this.httpClient.BaseAddress = new Uri(url);
    }
    public async Task<User?> GetDefectDojoUserByUsername(string applicationOwner)
    {
        var url = QueryStringHelper.BuildUrlWithQueryStringUsingStringConcat(
            "users/",
            new Dictionary<string, string> { { "username", applicationOwner } });
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
        return DefectDojoApiDeserializer<User>.DeserializeFirstItemOfResults(await response.Content.ReadAsStringAsync());
    }

    public async Task<ProductType?> GetProductTypeByNameAsync(string productTypeName)
    {
        var url = QueryStringHelper.BuildUrlWithQueryStringUsingStringConcat("product_types/", new Dictionary<string, string>()
        {
            { "name", productTypeName }
        });
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
        return DefectDojoApiDeserializer<ProductType>.DeserializeFirstItemOfResults(await response.Content.ReadAsStringAsync());
    }

    public async Task<Product> CreateProductAsync(string projectInfoName, string description, int productType, Lifecycle? lifecycle,
        int? applicationOwnerId, int? applicationOwnerBackUpId, int? functionalOwnerId,
        int? numberOfUsers, bool openToPartner = false)
    {
        var body = new
        {
            name = projectInfoName,
            description,
            prod_type = productType,
            team_manager = applicationOwnerBackUpId,
            technical_contact = applicationOwnerId,
            product_manager = functionalOwnerId,
            user_records = numberOfUsers,
            external_audience = openToPartner,
            lifecycle = lifecycle != null ? lifecycle.ToString() : null
        };

        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("products/", content);
        
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while creating the Project. Status code : {(int)response.StatusCode} - {response.StatusCode}");
        
        return JObject.Parse(await response.Content.ReadAsStringAsync()).ToObject<Product>() ??
               throw new Exception($"New Product '{projectInfoName}' could not be retrieved");
    }
}