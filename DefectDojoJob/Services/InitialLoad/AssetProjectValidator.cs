using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.InitialLoad;

public class AssetProjectValidator : IAssetProjectValidator
{
    public void Validate(AssetProject project)
    {
        const string message = "Invalid project information - ";
        if (project.Id is < 0) throw new Exception(message+"Id has invalid value");
        if (string.IsNullOrEmpty(project.Code.Trim())) throw new Exception(message+"Code cannot be null or empty");
        if (string.IsNullOrEmpty(project.Name.Trim())) throw new Exception(message+"Name cannot be null or empty");
        if (string.IsNullOrEmpty(project.ShortDescription?.Trim())
            && string.IsNullOrEmpty(project.DetailedDescription?.Trim())) throw new Exception(message+"Either short or detailed description should be provided");
    }

    public bool ShouldBeProcessed(DateTimeOffset refDate, AssetProject project)
    {
        return project.Created > refDate || project.Updated > refDate;
    }
}