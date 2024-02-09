using DefectDojoJob.Models.DefectDojo;

namespace DefectDojoJob.Services;

public class ProcessingResult
{
    public TeamsProcessingResult TeamsProcessingResult { get; set; } = new();
    public UsersProcessingResult UsersProcessingResult { get; set; } = new();
    public ProductsProcessingResult ProductsProcessingResult { get; set; } = new();
}