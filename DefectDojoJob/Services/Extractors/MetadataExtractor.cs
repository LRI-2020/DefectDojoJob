using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Extractors;

public class MetadataExtractor : IMetadataExtractor
{
    public List<(Metadata metadata,bool required)> ExtractMetadata(AssetProject project, int productId)
    {
        if (productId <= 0)
            throw new ErrorAssetProjectProcessor($"Invalid productId provided for metadata processing : '{productId}'",
                project.Code, EntitiesType.Metadata);
        
        var res = new List<(Metadata metadata, bool required)>
        {
            (ConstructMetadata("assetCode", project.Code, productId),true)
        };

        if(project.YearOfCreation?.ToString() != null)
            res.Add((ConstructMetadata("yearOfCreation", project.YearOfCreation.ToString()!, productId),false));
        
        if(project.Id.ToString() != null)
            res.Add((ConstructMetadata("assetId", project.Id.ToString()!, productId),false));

        return res;
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