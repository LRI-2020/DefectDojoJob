using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class UsersProcessor : IUsersProcessor
{
    private readonly IDefectDojoConnector defectDojoConnector;

    public UsersProcessor(IDefectDojoConnector defectDojoConnector)
    {
        this.defectDojoConnector = defectDojoConnector;
    }
    public async Task<List<UserProcessingResult>> ProcessUsersAsync(List<string> userNames)
    {
        var res = new List<UserProcessingResult>();
        foreach (var userName in userNames)
        {
            var userProcessResult = new UserProcessingResult();
            try
            {
                userProcessResult = await ProcessUserAsync(userName);
            }
            catch (Exception e)
            {
                if (e is WarningAssetProjectInfoProcessor warning) userProcessResult.Warnings.Add(warning);
                else
                {
                    userProcessResult.Errors.Add(new ErrorAssetProjectInfoProcessor(e.Message, userName, EntityType.User));
                }
            }
            res.Add(userProcessResult);
        }
        return res;
    }

    public async Task<UserProcessingResult> ProcessUserAsync(string username)
    {
        var res = new UserProcessingResult();
        var user = await defectDojoConnector.GetDefectDojoUserByUsernameAsync(username);
        if (user == null)
            throw new WarningAssetProjectInfoProcessor(
                $"Warning : user {username} does not exist in Defect Dojo", username, EntityType.User);
        res.Entity= new AssetToDefectDojoMapper(username, user.Id, EntityType.User);
        return res;

    }

}