using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Results;

namespace DefectDojoJob.Services.Interfaces;

public interface IUsersProcessor
{
    public Task<List<UserProcessingResult>> ProcessUsersAsync(List<string> userNames);
    public Task<UserProcessingResult> ProcessUserAsync(string username);

}