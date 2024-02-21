using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Services.Interfaces;

public interface IProductsProcessor
{
    public Task<List<ProductProcessingResult>> ProcessProductsAsync(List<AssetProject> projects,
        List<AssetToDefectDojoMapper> users);

    public Task<ProductProcessingResult> ProcessProductAsync(AssetProject project,
        List<AssetToDefectDojoMapper> users, ProductAdapterAction requiredAction, int? productId);
}