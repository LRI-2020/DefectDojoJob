using Newtonsoft.Json;

namespace DefectDojoJob.Models.DefectDojo;

public class DojoGroup
{
    private readonly string name;

    [JsonProperty(Required = Required.Always)]
    public int Id { get; set; }
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }
    public List<int> Users { get; set; } = new();

    public DojoGroup(string name)
    {
        this.Name = name;
    }
}