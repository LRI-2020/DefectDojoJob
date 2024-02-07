using Newtonsoft.Json;

namespace DefectDojoJob.Models.DefectDojo;

public class DojoGroup
{
    [JsonProperty(Required = Required.Always)]
    public int Id { get; set; }
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }
    public List<int> Users { get; set; } = new();
}