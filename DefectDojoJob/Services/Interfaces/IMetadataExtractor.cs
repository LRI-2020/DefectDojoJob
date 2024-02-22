using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IMetadataExtractor
{
    public List<(Metadata metadata,bool required)> ExtractMetadata(AssetProject project, int productId);
}