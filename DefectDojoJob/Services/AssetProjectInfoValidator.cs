using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Interfaces;

namespace DefectDojoJob.Services;

public class AssetProjectInfoValidator : IAssetProjectInfoValidator
{
    public void Validate(AssetProjectInfo projectInfo)
    {
        const string message = "Invalid project information - ";
        if (projectInfo.Id < 0) throw new Exception(message+"Id has invalid value");
        if (string.IsNullOrEmpty(projectInfo.Name.Trim())) throw new Exception(message+"Name cannot be null or empty");
        if (string.IsNullOrEmpty(projectInfo.ShortDescription?.Trim())
            && string.IsNullOrEmpty(projectInfo.DetailedDescription?.Trim())) throw new Exception(message+"Either short or detailed description should be provided");
    }

    public bool ShouldBeProcessed(DateTimeOffset refDate, AssetProjectInfo projectInfo)
    {
        return projectInfo.Created > refDate || projectInfo.Updated > refDate;
    }
}