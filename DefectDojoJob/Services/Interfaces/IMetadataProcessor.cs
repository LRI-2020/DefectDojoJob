using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IMetadataProcessor
{
    public Task<MetadataProcessingResult> ProcessProjectMetadata(AssetProject project);
}