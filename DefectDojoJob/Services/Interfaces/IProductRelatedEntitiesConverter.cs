using DefectDojoJob.Models.Extractions;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IProductRelatedEntitiesConverter
{
    public ProductAdapterResult ConvertProductRelatedEntities(AssetProject project);
}