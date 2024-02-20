using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public class ProductsProcessingResult
{
    public List<ProductProcessingResult> ProductsProcessingResults { get; set; }
}