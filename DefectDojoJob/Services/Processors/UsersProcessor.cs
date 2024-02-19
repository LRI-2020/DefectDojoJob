using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class UsersProcessor : IUsersProcessor
{
    private readonly IDefectDojoConnector defectDojoConnector;

    public UsersProcessor(IDefectDojoConnector defectDojoConnector)
    {
        this.defectDojoConnector = defectDojoConnector;
    }
    public async Task<UsersProcessingResult> ProcessUsersAsync(List<string> userNames)
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
                if (e is WarningAssetProjectInfoProcessor warning) res.Warnings.Add(warning);
                else
                {
                    res.Errors.Add(new ErrorAssetProjectInfoProcessor(e.Message, userName, EntityType.User));
                }
            }
        }

        return res;
    }

    public async Task<AssetToDefectDojoMapper> ProcessUserAsync(string username)
    {
        var user = await defectDojoConnector.GetDefectDojoUserByUsernameAsync(username);
        if (user != null)
        {
            return new AssetToDefectDojoMapper(username, user.Id);
        }

        throw new WarningAssetProjectInfoProcessor($"Warning : user {username} does not exist in Defect Dojo", username, EntityType.User);
    }

}