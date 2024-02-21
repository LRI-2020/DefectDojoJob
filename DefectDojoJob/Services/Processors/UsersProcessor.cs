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
                if (e is WarningAssetProjectProcessor warning) res.Warnings.Add(warning);
                else
                {
                    res.Errors.Add(new ErrorAssetProjectProcessor(e.Message, userName, EntitiesType.User));
                }
            }
        }
        return res;
    }

    public async Task<AssetToDefectDojoMapper> ProcessUserAsync(string username)
    {
        var user = await defectDojoConnector.GetDefectDojoUserByUsernameAsync(username);
        if (user == null)
            throw new WarningAssetProjectProcessor(
                $"Warning : user {username} does not exist in Defect Dojo", username, EntitiesType.User);
        return new AssetToMetadataMapper(user.UserName,user.Id);

    }

}