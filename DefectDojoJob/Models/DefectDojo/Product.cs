using DefectDojoJob.Services;
using Newtonsoft.Json;

namespace DefectDojoJob.Models.DefectDojo;

public class Product
{
    public Product(string name, string description)
    {
        Name = name;
        Description = description;
    }

    [JsonProperty(Required = Required.Always)]
    public int Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Description { get; set; }

    [JsonProperty("prod_type", Required = Required.Always)]
    public int ProductTypeId { get; set; }

    [JsonProperty("product_meta")] public List<ProductMeta> ProductMetas { get; set; } = new();
    public DateTime? Created { get; }
    public Lifecycle Lifecycle { get; set; }
    [JsonProperty("user_records")] public int? UserRecords { get; set; }
    [JsonProperty("external_audience")] public bool? ExternalAudience { get; set; }
    [JsonProperty("product_manager")] public int? ProductManager { get; set; }
    [JsonProperty("technical_contact")] public int? TechnicalContact { get; set; }
    [JsonProperty("team_manager")] public int? TeamManager { get; set; }
}