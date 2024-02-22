using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IMetadataProcessor
{
    Task<MetadataProcessingResult> ProcessProjectMetadataAsync(AssetProject project, ProductAdapterAction action, int productId);
}