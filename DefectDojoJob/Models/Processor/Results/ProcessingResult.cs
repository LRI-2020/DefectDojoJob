using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Services;

namespace DefectDojoJob.Models.Processor.Results;

public class ProcessingResult
{
    public UsersProcessingResult UsersProcessingResult { get; set; } 
    public DojoGroupsProcessingResult DojoGroupsProcessingResult { get; set; } 
    public List<ProductProcessingResult> ProductsProcessingResults { get; set; } = new();
    public List<string> GeneralErrors { get; set; } = new();
}