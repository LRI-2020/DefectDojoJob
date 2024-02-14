using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IProductsProcessor
{
    public Task<ProductsProcessingResult> ProcessProductsAsync(List<AssetProjectInfo> projects,
        List<AssetToDefectDojoMapper> users);

    public Task<AssetToDefectDojoMapper> ProcessProduct(AssetProjectInfo projectInfo,
        List<AssetToDefectDojoMapper> users);
}