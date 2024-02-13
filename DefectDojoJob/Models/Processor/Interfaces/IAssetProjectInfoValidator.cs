namespace DefectDojoJob.Models.Processor.Interfaces;

public interface IAssetProjectInfoValidator
{

    void Validate(AssetProjectInfo projectInfo);
    bool ShouldBeProcessed(DateTimeOffset refDate, AssetProjectInfo projectInfo);
}