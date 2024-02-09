using DefectDojoJob.Models.DefectDojo;

namespace DefectDojoJob.Models.Processor;

public class Extraction
{
    public HashSet<string> Users { get; set; } = new();
    public HashSet<string> Teams { get; set; } = new();

    public HashSet<Product> Products { get; set; } = new();
}