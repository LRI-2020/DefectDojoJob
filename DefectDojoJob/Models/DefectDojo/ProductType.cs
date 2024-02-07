using Newtonsoft.Json;

namespace DefectDojoJob.Models.DefectDojo;

public class ProductType
{
    public ProductType(int id, DateTime updated, DateTime created, string name)
    {
        Id = id;
        Updated = updated;
        Created = created;
        Name = name;
    }

    [JsonProperty(Required = Required.Always)]
    public int Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }

    public string? Description { get; set; }
    public DateTime Updated { get; }
    public DateTime Created { get; }
}