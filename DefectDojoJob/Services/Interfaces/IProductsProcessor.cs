using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Services.Interfaces;

public interface IProductsProcessor
{
    public Task<List<ProductProcessingResult>> ProcessProductsAsync(List<AssetProjectInfo> projects,
        List<AssetToDefectDojoMapper> users);

    public Task<ProductProcessingResult> ProcessProductAsync(AssetProjectInfo projectInfo,
        List<AssetToDefectDojoMapper> users, AssetProjectInfoProcessingAction requiredAction, int? productId);
}