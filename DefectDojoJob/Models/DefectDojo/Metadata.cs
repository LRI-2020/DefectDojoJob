using Newtonsoft.Json;

namespace DefectDojoJob.Models.DefectDojo;

public class Metadata
{
    [JsonProperty(Required = Required.Always)]
    public int Id { get; set; }

    [JsonProperty(Required = Required.Always)]

    public int Product { get; set; }

    [JsonProperty(Required = Required.Always)]

    public string Name { get; set; }

    [JsonProperty(Required = Required.Always)]

    public string Value { get; set; }
}