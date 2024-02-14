using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IEntitiesExtractor
{
    Extraction ExtractEntities(List<AssetProjectInfo> assetProjectInfos);
}