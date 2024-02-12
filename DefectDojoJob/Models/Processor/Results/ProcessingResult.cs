using DefectDojoJob.Services;

namespace DefectDojoJob.Models.Processor.Results;

public class ProcessingResult
{
    public UsersProcessingResult UsersProcessingResult { get; set; } = new();
    public ProductsProcessingResult ProductsProcessingResult { get; set; } = new();
}