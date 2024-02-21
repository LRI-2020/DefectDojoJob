using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IUsersExtractor
{
    public HashSet<string> ExtractValidUsernames(AssetProject p);
}