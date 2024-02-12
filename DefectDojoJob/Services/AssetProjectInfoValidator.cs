using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services;

public class AssetProjectInfoValidator
{
    public void Validate(AssetProjectInfo projectInfo)
    {
        if (projectInfo.Id < 0) throw new Exception("Id has invalid value");
        if (string.IsNullOrEmpty(projectInfo.Name.Trim())) throw new Exception("Name cannot be null or empty");
        if (string.IsNullOrEmpty(projectInfo.ShortDescription?.Trim())
            && string.IsNullOrEmpty(projectInfo.DetailedDescription?.Trim())) throw new Exception("Either short or detailed description should be provided");
    }

    public bool ShouldBeProcessed(DateTimeOffset refDate, AssetProjectInfo projectInfo)
    {
        return projectInfo.Created > refDate || projectInfo.Updated > refDate;
    }
}