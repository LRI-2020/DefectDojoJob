using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IProductExtractor
{
    public Task<Product> ExtractProduct(AssetProject project, List<AssetToDefectDojoMapper> users);

}