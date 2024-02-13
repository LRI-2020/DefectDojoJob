using Newtonsoft.Json.Linq;

namespace DefectDojoJob.Models.Processor.Results;

public class InitialLoadResult
{

    public List<AssetProjectInfo> DiscardedProjects { get;  } = new();
    public List<AssetProjectInfo> ProjectsToProcess { get;  } = new();
    public  List<(JObject? jObject, string error)> Errors { get; } = new();
}