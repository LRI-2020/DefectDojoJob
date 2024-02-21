using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Adapters;

public interface IProjectsAdapter
{
    public Task<List<ProductAdapterResult>> StartAdapterAsync(List<AssetProject> projects, List<AssetToDefectDojoMapper> users);
}