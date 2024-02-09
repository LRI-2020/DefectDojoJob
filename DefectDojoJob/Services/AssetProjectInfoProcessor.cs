using DefectDojoJob.Models;

namespace DefectDojoJob.Services;

public class AssetProjectInfoProcessor
{
    private readonly DefectDojoConnector defectDojoConnector;

    public AssetProjectInfoProcessor(DefectDojoConnector defectDojoConnector)
    {
        this.defectDojoConnector = defectDojoConnector;
    }

    public async Task<ProcessingResult> StartProcessingAsync(List<AssetProjectInfo> assetProjectInfos)
    {
        var processingResult = new ProcessingResult();
        var teams = ExtractTeams(assetProjectInfos).ToList();
        if (teams.Any()) processingResult.TeamsResult = await TeamsProcessorAsync(teams);

        var users = ExtractUsers(assetProjectInfos).ToList();
        if (users.Any()) processingResult.UsersResult = await UsersProcessorAsync(users);

        return processingResult;
    }

    private async Task<List<UserEntityProcessingResult>> UsersProcessorAsync(List<string> userNames)
    {
        var res = new List<UserEntityProcessingResult>();
        foreach (var userName in userNames)
        {
            res.Add(await ProcessUserAsync(userName));
        }

        return res;
    }

    private async Task<List<TeamEntityProcessingResult>> TeamsProcessorAsync(List<string> teamNames)
    {
        var res = new List<TeamEntityProcessingResult>();
        foreach (var teamName in teamNames)
        {
            res.Add(await ProcessTeamAsync(teamName));
        }

        return res;
    }

    private async Task<TeamEntityProcessingResult> ProcessTeamAsync(string teamName)
    {
        var res = new TeamEntityProcessingResult
        {
            AssetIdentifier = teamName,
            ProcessingSuccessful = true
        };

        try
        {
            var team = await defectDojoConnector.GetDefectDojoGroupByNameAsync(teamName);
            if (team != null) res.DefectDojoId = team.Id;
            else
            {
                res.Warnings.Add($"Team {teamName} not found in Defect Dojo");
                res.DefectDojoId = -1;
            }
        }
        catch (Exception e)
        {
            res.Errors.Add(e.Message);
            res.ProcessingSuccessful = false;
        }

        return res;
    }

    private IEnumerable<string> ExtractUsers(List<AssetProjectInfo> assetProjectInfos)
    {
        var applicationOwners = assetProjectInfos
            .Where(t => !string.IsNullOrEmpty(t.ApplicationOwner?.Trim()))
            .Select(a => a.ApplicationOwner);

        var applicationOwnersBP = assetProjectInfos
            .Where(t => !string.IsNullOrEmpty(t.ApplicationOwnerBackUp?.Trim()))
            .Select(a => a.ApplicationOwnerBackUp);

        var functionalOwners = assetProjectInfos
            .Where(t => !string.IsNullOrEmpty(t.FunctionalOwner?.Trim()))
            .Select(a => a.FunctionalOwner);

        return applicationOwners.Concat(applicationOwnersBP).Concat(functionalOwners).Distinct()!;
    }

    private IEnumerable<string> ExtractTeams(IEnumerable<AssetProjectInfo> assetProjectInfos)
    {
        return assetProjectInfos
            .Where(t => !string.IsNullOrEmpty(t.Team?.Trim()))
            .Select(a => a.Team)
            .Distinct()!;
    }


    private async Task<UserEntityProcessingResult> ProcessUserAsync(string username)
    {
        var res = new UserEntityProcessingResult { ProcessingSuccessful = true };
        try
        {
            var user = await defectDojoConnector.GetDefectDojoUserByUsername(username);
            if (user != null) res.DefectDojoId = user.Id;
            else
            {
                res.DefectDojoId = -1;
                res.Warnings.Add($"Warning : user {username} does not exist in Defect Dojo");
                res.ProcessingSuccessful = false;
            }
        }
        catch (Exception e)
        {
            res.Errors.Add(e.Message);
            res.ProcessingSuccessful = false;
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