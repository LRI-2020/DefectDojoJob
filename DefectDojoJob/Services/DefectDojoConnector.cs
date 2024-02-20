using System.Net.Http.Headers;
using System.Text;
using DefectDojoJob.Helpers;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Services;

public class DefectDojoConnector : IDefectDojoConnector
{
    private readonly HttpClient httpClient;

    public DefectDojoConnector(IConfiguration configuration, HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", configuration["ApiToken"]);

        var url = configuration["DefectDojoBaseUrl"];
        if (url != null) this.httpClient.BaseAddress = new Uri(url);
    }

    public async Task<User?> GetDefectDojoUserByUsernameAsync(string applicationOwner)
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
        var content = GenerateProductBody(product, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("products/", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while creating the Project. Status code : {(int)response.StatusCode} - {response.StatusCode}");

        return JObject.Parse(await response.Content.ReadAsStringAsync()).ToObject<Product>() ??
               throw new Exception($"New Product '{product.Name}' could not be retrieved");
    }

    public async Task<Metadata?> GetMetadataAsync(Dictionary<string, string> searchParams)
    {
        var url = QueryStringHelper.BuildUrlWithQueryStringUsingStringConcat(
            "metadata/",
            searchParams);
        var response = await httpClient.GetAsync(url);
        if ((int)response.StatusCode == 404) return null;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
        return DefectDojoApiDeserializer<Metadata>.DeserializeFirstItemOfResults(await response.Content.ReadAsStringAsync());
    }

    public async Task<Product?> GetProductByNameAsync(string name)
    {
        var url = QueryStringHelper.BuildUrlWithQueryStringUsingStringConcat("products/",
            new Dictionary<string, string>()
            {
                { "name", name }
            });
        var response = await httpClient.GetAsync(url);
        if ((int)response.StatusCode == 404) return null;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
        return DefectDojoApiDeserializer<Product>.DeserializeFirstItemOfResults(await response.Content.ReadAsStringAsync());
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        var content = GenerateProductBody(product, Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"products/{product.Id}", content);
        if ((int)response.StatusCode == 404)
            throw new ErrorAssetProjectInfoProcessor(
                $"No product with Id {product.Id} found, update could not be processed", product.Name, EntityType.Product);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while updating the Project. Status code : {(int)response.StatusCode} - {response.StatusCode}");

        return JObject.Parse(await response.Content.ReadAsStringAsync()).ToObject<Product>() ??
               throw new Exception($"Updated Product '{product.Name}' could not be retrieved");
    }

    public async Task<Metadata> CreateMetadataAsync(Metadata metadata)
    {
        var body = new
        {
            name = metadata.Name,
            value = metadata.Value,
            product = metadata.Product
        };

        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("metadata/", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while creating the Metadata. Status code : {(int)response.StatusCode} - {response.StatusCode}");

        return JObject.Parse(await response.Content.ReadAsStringAsync()).ToObject<Metadata>() ??
               throw new Exception($"New Metadata '{metadata.Name}' could not be retrieved");
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var response = await httpClient.DeleteAsync($"products/{productId}");
        return response.IsSuccessStatusCode;
    }

    private StringContent GenerateProductBody(Product product, Encoding encoding, string mediaType)
    {
        var body = new
        {
            name = product.Name,
            description = product.Description,
            prod_type = product.ProductTypeId,
            team_manager = product.TeamManager,
            technical_contact = product.TechnicalContact,
            product_manager = product.ProductManager,
            user_records = product.UserRecords,
            external_audience = product.ExternalAudience,
            lifecycle = product.Lifecycle?.ToString()
        };

        return new StringContent(JsonConvert.SerializeObject(body), encoding, mediaType);
    }

    private async Task<Product?> GetProductByIdAsync(int productId)
    {
        var url = $"products/{productId}";
        var response = await httpClient.GetAsync(url);
        if ((int)response.StatusCode == 404) return null;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error while processing the request, status code : {(int)response.StatusCode} - {response.StatusCode}");
        return DefectDojoApiDeserializer<Product>.DeserializeFirstItemOfResults(await response.Content.ReadAsStringAsync());
    }
}