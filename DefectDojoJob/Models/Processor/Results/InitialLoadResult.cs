using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Models.Processor.Results;

public class InitialLoadResult
{

    public List<AssetProject> DiscardedProjects { get;  } = new();
    public List<AssetProject> ProjectsToProcess { get;  } = new();
    public  List<(JObject? jObject, string error)> Errors { get; } = new();
}