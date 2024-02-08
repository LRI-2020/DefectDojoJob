using System.ComponentModel;
using System.Net.Http.Json;
using System.Net.Security;
using System.Text.Json;
using DefectDojoJob.Models;
using DefectDojoJob.Models.DefectDojo;

namespace DefectDojoJob.Services;

public class AssetProjectInfoProcessor
{
    private readonly DefectDojoConnector defectDojoConnector;

    public AssetProjectInfoProcessor(DefectDojoConnector defectDojoConnector)
    {
        this.defectDojoConnector = defectDojoConnector;
    }

    public async Task<AssetProjectInfoProcessingResult> ProcessAssetProjectInfo(AssetProjectInfo assetProjectInfo)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        var result = new AssetProjectInfoProcessingResult()
        {
            AssetId = assetProjectInfo.Id
        };
        try
        {
            result.TeamId = await TeamProcessorAsync(assetProjectInfo);
            result.UserIds.Add(await ProcessUserAsync(nameof(assetProjectInfo.ApplicationOwner),
                assetProjectInfo.ApplicationOwner));
            result.UserIds.Add(await ProcessUserAsync(nameof(assetProjectInfo.ApplicationOwnerBackUp),
                assetProjectInfo.ApplicationOwnerBackUp));
            result.UserIds.Add(await ProcessUserAsync(nameof(assetProjectInfo.FunctionalOwner),
                assetProjectInfo.FunctionalOwner));
        }
        catch (Exception e)
        {
            if (e is WarningAssetProjectInfoProcessor)
                warnings.Add(e.Message);
            else
            {
                errors.Add(e.Message);
            }
        }

        result.Errors = errors;
        result.Warnings = warnings;
        return result;
    }

    private async Task<int> TeamProcessorAsync(AssetProjectInfo assetProjectInfo)
    {
        if (string.IsNullOrEmpty(assetProjectInfo.Team)) throw new WarningAssetProjectInfoProcessor("No Team provided");
        var team = await defectDojoConnector.GetDefectDojoGroupByNameAsync(assetProjectInfo.Team);
        if (team != null) return team.Id;
        return await defectDojoConnector.CreateDojoGroup(assetProjectInfo.Team);
    }

    //Process users in single method but then, cannot have specific errors or warning messages
    private async Task<List<int>> UsersProcessorAsync(AssetProjectInfo assetProjectInfo)
    {
        var res = new List<int>();

        var users = new Dictionary<string, string?>()
        {
            { nameof(assetProjectInfo.ApplicationOwner), assetProjectInfo.ApplicationOwner },
            { nameof(assetProjectInfo.ApplicationOwnerBackUp), assetProjectInfo.ApplicationOwnerBackUp },
            { nameof(assetProjectInfo.FunctionalOwner), assetProjectInfo.FunctionalOwner }
        };

        foreach (var kvp in users)
        {
            res.Add(await ProcessUserAsync(kvp.Key,kvp.Value));
        }

        return res;
    }

    private async Task<int> ProcessUserAsync(string key, string? username)
    {
        if (string.IsNullOrEmpty(username)) throw new WarningAssetProjectInfoProcessor($"No {key} provided");
        var user = await defectDojoConnector.GetDefectDojoUserByUsername(username);
        if (user != null) return user.Id;
        return await defectDojoConnector.CreateDojoUser(username);
    }

    private bool ProductProcessor(AssetProjectInfo assetProjectInfo)
    {
        return false;
    }
}