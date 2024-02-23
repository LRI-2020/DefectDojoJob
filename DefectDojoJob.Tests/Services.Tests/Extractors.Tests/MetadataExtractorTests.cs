namespace DefectDojoJob.Tests.Services.Tests.Extractors.Tests;

public class MetadataExtractorTests
{
    [Theory]
    [InlineAutoMoqData(0)]
    [InlineAutoMoqData(-1)]
    public void WhenProductIdInvalid_ErrorThrown(int productId,MetadataExtractor sut, AssetProject pi)
    {

        Func<List<(Metadata metadata, bool required)>> act = () => sut.ExtractMetadata(pi, productId);
        act.Should().Throw<ErrorAssetProjectProcessor>()
            .Where(e => e.Message
                .Contains($"Invalid productId provided for metadata processing : '{productId}'"));
    }
    [Theory]
    [AutoMoqData]
    public void WhenExtract_MetadataCodeAlwaysInRes(int productId,MetadataExtractor sut, AssetProject pi)
    {
        var res = sut.ExtractMetadata(pi, productId);
        res.Where(r => r.metadata.Name.ToUpper() == "ASSETCODE")
            .Should().Contain(r => r.metadata.Value == pi.Code);
    }

    [Theory]
    [AutoMoqData]
    public void WhenExtract_AllMetadataNotNullInResult(int assetId,int year, int productId,MetadataExtractor sut, AssetProject pi)
    {
        pi.YearOfCreation = year;
        pi.Id = assetId;
        var res = sut.ExtractMetadata(pi, productId);
        res.Where(r => r.metadata.Name.ToUpper() == "YEAROFCREATION")
            .Should().Contain(r => r.metadata.Value == year.ToString()); 
        res.Where(r => r.metadata.Name.ToUpper() == "ASSETID")
            .Should().Contain(r => r.metadata.Value == assetId.ToString());
    }

    [Theory]
    [AutoMoqData]
    public void WhenExtract_NullMetadataDiscarded(int productId,MetadataExtractor sut, AssetProject pi)
    {
        pi.YearOfCreation = null;
        pi.Id = null;
        var res = sut.ExtractMetadata(pi, productId);
        res.Should().NotContain(r => r.metadata.Name.ToUpper() == "YEAROFCREATION");
        res.Should().NotContain(r => r.metadata.Name.ToUpper() == "ID");
    }
}