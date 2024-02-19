using Newtonsoft.Json;

namespace DefectDojoJob.Models.Processor;

public class AssetProjectInfo
{
    public int? Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }    
    
    [JsonProperty(Required = Required.Always)]
    public DateTimeOffset Created { get; set; }    
    
    [JsonProperty(Required = Required.Always)]
    public DateTimeOffset Updated { get; set; }

    public string?  ProductType { get; set; }

    public string? ShortDescription { get; set; }
    public string? DetailedDescription { get; set; }
    [JsonProperty(Required = Required.Always)]

    public string Code { get; set; }
    public string? State { get; set; }
    public string? Team { get; set; }
    public string? ApplicationOwner { get; set; }
    public string? ApplicationOwnerBackUp { get; set; }
    public string? FunctionalOwner { get; set; }
    public List<string>? BusinessContact { get; set; }
    public int? YearOfCreation { get; set; }
    public int? NumberOfUsers { get; set; }
    public string? AccessInformation { get; set; }
    public bool? OpenToPartner { get; set; }
    public List<string>? ContactPersons { get; set; }
    public List<string>? Urls { get; set; }
    public string? UrlType { get; set; }
    public string? Url { get; set; }
}