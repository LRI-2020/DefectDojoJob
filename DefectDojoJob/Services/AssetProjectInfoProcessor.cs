using System.ComponentModel;
using DefectDojoJob.Models;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;

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
        var extraction = ExtractEntities(assetProjectInfos);
        if (extraction.Teams.Any()) processingResult.TeamsProcessingResult = await TeamsProcessorAsync(extraction.Teams.ToList());
        if (extraction.Users.Any()) processingResult.UsersProcessingResult = await UsersProcessorAsync(extraction.Users.ToList());

        processingResult.ProductsProcessingResult = await ProductsProcessorAsync(assetProjectInfos,
            processingResult.TeamsProcessingResult.Entities,
            processingResult.UsersProcessingResult.Entities);

        return processingResult;
    }

    private Extraction ExtractEntities(List<AssetProjectInfo> assetProjectInfos)
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

    private List<string> ExtractUsers(AssetProjectInfo p)
    {
        var res = new List<string>();
        if (!string.IsNullOrEmpty(p.ApplicationOwner?.Trim())) res.Add(p.ApplicationOwner);
        if (!string.IsNullOrEmpty(p.ApplicationOwnerBackUp?.Trim())) res.Add(p.ApplicationOwnerBackUp);
        if (!string.IsNullOrEmpty(p.FunctionalOwner?.Trim())) res.Add(p.FunctionalOwner);
        return res;
    }

    private async Task<UsersProcessingResult> UsersProcessorAsync(List<string> userNames)
    {
        var res = new UsersProcessingResult();
        foreach (var userName in userNames)
        {
            try
            {
                res.Entities.Add(await ProcessUserAsync(userName));

            }
            catch (Exception e)
            {
                if (e is WarningException) res.Warnings.Add(e.Message);
                else
                {
                    res.Errors.Add(e.Message);
                }
            }
        }

        return res;
    }

    private async Task<TeamsProcessingResult> TeamsProcessorAsync(List<string> teamNames)
    {
        var res = new TeamsProcessingResult();
        foreach (var teamName in teamNames)
        {
            try
            {
                res.Entities.Add(await ProcessTeamAsync(teamName));
            }
            catch (Exception e)
            {
                if (e is WarningException) res.Warnings.Add(e.Message);
                else
                {
                    res.Errors.Add(e.Message);
                }
            }
        }

        return res;
    }

    private async Task<(string AssetIdentifier, int DefectDojoId)> ProcessTeamAsync(string teamName)
    {
        var team = await defectDojoConnector.GetDefectDojoGroupByNameAsync(teamName);
        if (team != null)
        {
            return (teamName, team.Id);
        }

        throw new WarningException($"Team {teamName} not found in Defect Dojo");
    }


    private async Task<(string AssetIdentifier, int DefectDojoId)> ProcessUserAsync(string username)
    {
        var user = await defectDojoConnector.GetDefectDojoUserByUsername(username);
        if (user != null)
        {
            return (username, user.Id);
        }

        throw new WarningException($"Warning : user {username} does not exist in Defect Dojo");
    }

    private async Task<ProductsProcessingResult> ProductsProcessorAsync(List<AssetProjectInfo> projects, 
        List<(string AssetIdentifier,int DefectDojoId)> teams, 
        List<(string AssetIdentifier,int DefectDojoId)> users)
    {
        var res = new ProductsProcessingResult();
        foreach (var project in projects)
        {
            await ProcessProduct(project, teams, users);
        }

        return res;
    }

    private async Task<(string AssetIdentifier, int DefectDojoId)> ProcessProduct(AssetProjectInfo projectInfo,
        List<(string AssetIdentifier,int DefectDojoId)> teams, 
        List<(string AssetIdentifier,int DefectDojoId)> users)
    {
        //Create Product - stop if error;
        var description = projectInfo.ShortDescription + projectInfo.DetailedDescription;
        await defectDojoConnector.CreateProductAsync(projectInfo.Name, description ?? "Enter a description");

        //LInk product to team
        //Link product to users
        //Link product to
    }
}