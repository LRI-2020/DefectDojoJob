using System.Net.Http.Headers;
using System.Text;
using DefectDojoJob.Helpers;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Services;

public class DefectDojoConnector:IDefectDojoConnector
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

    public async Task<Product> CreateProductAsync(Product product)
    {
        var body = new
        {
            name = product.Name,
            description=product.Description,
            prod_type = product.ProductTypeId,
            team_manager = product.TeamManager,
            technical_contact = product.TechnicalContact,
            product_manager = product.ProductManager,
            user_records = product.UserRecords,
            external_audience = product.ExternalAudience,
            lifecycle = product.Lifecycle != null ? product.Lifecycle.ToString() : null
        };

        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("products/", content);
        
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while creating the Project. Status code : {(int)response.StatusCode} - {response.StatusCode}");
        
        return JObject.Parse(await response.Content.ReadAsStringAsync()).ToObject<Product>() ??
               throw new Exception($"New Product '{product.Name}' could not be retrieved");
    }
}