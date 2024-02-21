using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Extractors;

public class UsersExtractor : IUsersExtractor
{
    public HashSet<string> ExtractValidUsernames(AssetProject p)
    {
        var res = new HashSet<string>();
        if (!string.IsNullOrEmpty(p.ApplicationOwner?.Trim())) res.Add(p.ApplicationOwner);
        if (!string.IsNullOrEmpty(p.ApplicationOwnerBackUp?.Trim())) res.Add(p.ApplicationOwnerBackUp);
        if (!string.IsNullOrEmpty(p.FunctionalOwner?.Trim())) res.Add(p.FunctionalOwner);
        return res;
    }
}