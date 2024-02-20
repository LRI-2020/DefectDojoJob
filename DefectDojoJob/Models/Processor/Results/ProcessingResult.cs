using DefectDojoJob.Services;

namespace DefectDojoJob.Models.Processor.Results;

public class ProcessingResult
{
    public List<UserProcessingResult> UsersProcessingResult { get; set; } = new();
    public List<ProductProcessingResult> ProductsProcessingResults { get; set; } = new();
}