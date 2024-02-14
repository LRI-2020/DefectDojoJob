using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class EntitiesExtractor : IEntitiesExtractor
{
    public Extraction ExtractEntities(List<AssetProjectInfo> assetProjectInfos)
    {
        var res = new Extraction();
        assetProjectInfos.ForEach(p =>
        {
            if (!string.IsNullOrEmpty(p.Team?.Trim())) res.Teams.Add(p.Team);

            var users = ExtractUsers(p);
            if (users.Count > 0) users.ForEach(i => res.Users.Add(i));
        });

        return res;
    }

    private static List<string> ExtractUsers(AssetProjectInfo p)
    {
        var res = new List<string>();
        if (!string.IsNullOrEmpty(p.ApplicationOwner?.Trim())) res.Add(p.ApplicationOwner);
        if (!string.IsNullOrEmpty(p.ApplicationOwnerBackUp?.Trim())) res.Add(p.ApplicationOwnerBackUp);
        if (!string.IsNullOrEmpty(p.FunctionalOwner?.Trim())) res.Add(p.FunctionalOwner);
        return res;
    }
}