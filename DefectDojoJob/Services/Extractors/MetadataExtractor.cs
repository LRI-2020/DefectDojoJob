using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Extractors;

public class MetadataExtractor: IMetadataExtractor

{
    public List<Metadata> ExtractMetadata(AssetProject project)
    {
        return new List<Metadata>();
    }
    private static Metadata ConstructMetadata(string name, string value, int productId)
    {
        return new Metadata
        {
            Name = name,
            Product = productId,
            Value = value
        };
    }
    
}

public interface IMetadataExtractor
{
    public List<Metadata> ExtractMetadata(AssetProject project);
}