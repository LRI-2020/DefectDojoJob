using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Services.Interfaces;

public interface IUsersProcessor
{
    public Task<UsersProcessingResult> ProcessUsersAsync(List<string> userNames);
    public Task<AssetToDefectDojoMapper> ProcessUserAsync(string username);

}