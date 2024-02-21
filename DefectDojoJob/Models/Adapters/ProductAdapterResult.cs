using DefectDojoJob.Models.DefectDojo;

namespace DefectDojoJob.Models.Extractions;

public class ProductAdapterResult
{
    public ProductAdapterResult(Product product)
    {
        Product = product;
    }

    public Product Product { get; set; }
    public HashSet<Metadata> MetadataSet { get; set; } = new();
    public HashSet<Engagement> Engagements { get; set; } = new();
    public HashSet<Endpoint> Endpoints { get; set; } = new();
}