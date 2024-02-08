using System.Net.Http.Json;
using System.Net.Security;
using System.Text.Json;
using DefectDojoJob.Models;

namespace DefectDojoJob.Services;

public class AssetProjectInfoProcessor
{
    private readonly DefectDojoConnector defectDojoConnector;

    public AssetProjectInfoProcessor(DefectDojoConnector defectDojoConnector)
    {
        this.defectDojoConnector = defectDojoConnector;
    }

    public async Task<AssetProjectInfoProcessingResult> ProccessAssetProjectInfo(AssetProjectInfo assetProjectInfo)
    {
        var errors = new List<string>();
        try
        {
            await TeamProcessorAsync(assetProjectInfo);

        }
        catch (Exception e)
        {
            errors.Add(e.Message);
        }

        return new AssetProjectInfoProcessingResult()
        {
            AssetId = assetProjectInfo.Id,
            Errors = errors
        };

    }
    private async Task<int> TeamProcessorAsync(AssetProjectInfo assetProjectInfo)
    {
        if (string.IsNullOrEmpty(assetProjectInfo.Team)) throw new Exception("No Team provided");
        var team = await defectDojoConnector.GetDefectDojoGroupByNameAsync(assetProjectInfo.Team);
        if (team != null) return team.Id;
        await defectDojoConnector.CreateDojoGroup(assetProjectInfo.Team);
        return -1;

    }

    

    private bool UsersProcessor(AssetProjectInfo assetProjectInfo)
    {
        return false;
    }

    private bool ProductProcessor(AssetProjectInfo assetProjectInfo)
    {
        return false;
    }
    
    
    
    
    
}