using Newtonsoft.Json;

namespace DefectDojoJob.Models.DefectDojo;

public class ProductMeta
{
    [JsonProperty(Required = Required.Always)]
    public int Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Value { get; set; }
}