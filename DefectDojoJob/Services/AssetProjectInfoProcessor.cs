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
        var result = new AssetProjectInfoProcessingResult
        {
            EntityId = assetProjectInfo.Id,
            TeamProcessingResult = await TeamProcessorAsync(assetProjectInfo),
            ApplicationOwnerProcessingResult = await ProcessUserAsync(nameof(assetProjectInfo.ApplicationOwner),
                assetProjectInfo.ApplicationOwner,null),
            ApplicationOwnerBUProcessingResult = await ProcessUserAsync(nameof(assetProjectInfo.ApplicationOwnerBackUp),
                assetProjectInfo.ApplicationOwnerBackUp,null),
            FunctionalOwnerProcessingResult = await ProcessUserAsync(nameof(assetProjectInfo.FunctionalOwner),
                assetProjectInfo.FunctionalOwner,null),
            //FullFill errors - concatenation?
            Errors = errors,
            Warnings = warnings
        };

        return result;
    }

    private async Task<TeamProcessingResult> TeamProcessorAsync(AssetProjectInfo assetProjectInfo)
    {
        var res = new TeamProcessingResult{ProcessingSuccessful = true};
        if (string.IsNullOrEmpty(assetProjectInfo.Team))
        {
            res.EntityId = -1;
            res.ProcessingSuccessful = false;
            res.Warnings.Add("No Team provided");
        }

        try
        {
            var team = await defectDojoConnector.GetDefectDojoGroupByNameAsync(assetProjectInfo.Team!)?? await defectDojoConnector.CreateDojoGroup(assetProjectInfo.Team);
            res.EntityId = team.Id;
        }
        catch (Exception e)
        {
            if (e is WarningAssetProjectInfoProcessor) res.Warnings.Add(e.Message);
            else
            {
                res.Errors.Add(e.Message);
                res.ProcessingSuccessful = false;
            }
        }

        return res;
    }

//Process users in single method but then, cannot have specific errors or warning messages
    private async Task<List<UserProcessingResult>> UsersProcessorAsync(AssetProjectInfo assetProjectInfo)
    {
        var res = new List<UserProcessingResult>();

        var users = new Dictionary<string, string?>()
        {
            { nameof(assetProjectInfo.ApplicationOwner), assetProjectInfo.ApplicationOwner },
            { nameof(assetProjectInfo.ApplicationOwnerBackUp), assetProjectInfo.ApplicationOwnerBackUp },
            { nameof(assetProjectInfo.FunctionalOwner), assetProjectInfo.FunctionalOwner }
        };

        foreach (var kvp in users)
        {
            res.Add(await ProcessUserAsync(kvp.Key, kvp.Value,null));
        }

        return res;
    }

    private async Task<UserProcessingResult> ProcessUserAsync(string key, string? username, int? teamId)
    {
        var res = new UserProcessingResult{ ProcessingSuccessful = true };
        if (string.IsNullOrEmpty(username))
        {
            res.Warnings.Add($"Warning : No {key} provided");
            res.ProcessingSuccessful = false;
            res.EntityId = -1;
            return res;
        }

        try
        {
            var user = await defectDojoConnector.GetDefectDojoUserByUsername(username) ?? await defectDojoConnector.CreateDojoUser(username);
            res.EntityId = user.Id;
        }
        catch (Exception e)
        {
            if (e is WarningAssetProjectInfoProcessor) res.Warnings.Add(e.Message);
            else
            {
                res.Errors.Add(e.Message);
                res.ProcessingSuccessful = false;
            }
        }

        return res;
    }

    private async Task<bool> AddUserToTeamAsync(int userId)
    {
        throw new NotImplementedException();
    }

    private bool ProductProcessor(AssetProjectInfo assetProjectInfo)
    {
        return false;
    }
}