using DefectDojoJob.Services;

namespace DefectDojoJob.Models.Processor.Results;

public class ProcessingResult
{
    public UsersProcessingResult UsersProcessingResult { get; set; } = new();
    public List<ProductProcessingResult> ProductsProcessingResult { get; set; } = new();
}