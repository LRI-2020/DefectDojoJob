using DefectDojoJob.Models;
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
        //will be usable if we must create team in DD and link user to team through group member object
        //if (extraction.Teams.Any()) processingResult.TeamsProcessingResult = await TeamsProcessorAsync(extraction.Teams.ToList());
        if (extraction.Users.Any()) processingResult.UsersProcessingResult = await UsersProcessorAsync(extraction.Users.ToList());

        processingResult.ProductsProcessingResult = await ProductsProcessorAsync(assetProjectInfos,
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
                if (e is WarningAssetProjectInfoProcessor warning) res.Warnings.Add(warning);
                else
                {
                    res.Errors.Add(new ErrorAssetProjectInfoProcessor(e.Message, userName, EntityType.User));
                }
            }
        }

        return res;
    }

    private async Task<AssetToDefectDojoMapper> ProcessUserAsync(string username)
    {
        var user = await defectDojoConnector.GetDefectDojoUserByUsername(username);
        if (user != null)
        {
            return new AssetToDefectDojoMapper(username, user.Id);
        }

        throw new WarningAssetProjectInfoProcessor($"Warning : user {username} does not exist in Defect Dojo", username, EntityType.User);
    }

    private async Task<ProductsProcessingResult> ProductsProcessorAsync(List<AssetProjectInfo> projects,
        List<AssetToDefectDojoMapper> users)
    {
        var result = new ProductsProcessingResult();
        foreach (var project in projects)
        {
            try
            {
                result.Entities.Add(await ProcessProduct(project, users));
            }
            catch (Exception e)
            {
                if (e is WarningAssetProjectInfoProcessor warning) result.Warnings.Add(warning);
                else
                {
                    result.Errors.Add(new ErrorAssetProjectInfoProcessor(e.Message, project.Name, EntityType.Product));
                }
            }
        }

        return result;
    }

    private async Task<AssetToDefectDojoMapper> ProcessProduct(AssetProjectInfo projectInfo,
        List<AssetToDefectDojoMapper> users)
    {
        //TODO ProductType Processing!!
        var description = $"Short Description : {projectInfo.ShortDescription}; Detailed Description : {projectInfo.DetailedDescription} ; ";
        var productType = 1;
        var lifecycle = MatchLifeCycle(projectInfo.State);

        var appOwnerId = users
            .Find(u => u.AssetIdentifier == projectInfo.ApplicationOwner)
            ?.DefectDojoId;
        var appOwnerBuId = users
            .Find(u => u.AssetIdentifier == projectInfo.ApplicationOwnerBackUp)!
            .DefectDojoId;
        var funcOwnerId = users
            .Find(u => u.AssetIdentifier == projectInfo.FunctionalOwner)!
            .DefectDojoId;

        var product = await defectDojoConnector.CreateProductAsync(projectInfo.Name, description ?? "Enter a description",
            productType, lifecycle, appOwnerId,
            appOwnerBuId, funcOwnerId,
            projectInfo.NumberOfUsers, projectInfo.OpenToPartner ?? false);

        return new AssetToDefectDojoMapper(projectInfo.Name, product.Id);
    }

    private Lifecycle? MatchLifeCycle(string? state)
    {
        if (string.IsNullOrEmpty(state)) return null;
        switch (state.Trim())
        {
            case "EnConstruction": return Lifecycle.construction;
            case "EnService":
            case "EnCoursDeDeclassement":
                return Lifecycle.production;
            case "Declassee": return Lifecycle.retirement;
            default: return null;
        }
    }
}