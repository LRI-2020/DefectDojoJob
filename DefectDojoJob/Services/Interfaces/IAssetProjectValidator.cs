using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IAssetProjectValidator
{

    void Validate(AssetProject project);
    bool ShouldBeProcessed(DateTimeOffset refDate, AssetProject project);
}