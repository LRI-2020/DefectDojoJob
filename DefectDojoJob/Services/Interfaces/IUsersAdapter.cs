using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.Processor;

namespace DefectDojoJob.Services.Interfaces;

public interface IUsersAdapter
{
    public Task<UsersAdaptersResults> StartUsersAdapterAsync(List<AssetProject> assetProjectInfos);
}