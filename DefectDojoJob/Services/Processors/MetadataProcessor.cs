using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class MetadataProcessor : IMetadataProcessor
{
    private readonly IDefectDojoConnector defectDojoConnector;
    private readonly IMetadataExtractor metadataExtractor;

    public MetadataProcessor(IDefectDojoConnector defectDojoConnector, IMetadataExtractor metadataExtractor)
    {
        this.defectDojoConnector = defectDojoConnector;
        this.metadataExtractor = metadataExtractor;
    }

    public async Task<MetadataProcessingResult> ProcessProjectMetadataAsync(AssetProject project, ProductAdapterAction action, int productId)
    {
        var res = new MetadataProcessingResult();

        var metadataInfoList = metadataExtractor.ExtractMetadata(project, productId);

        foreach (var metadataInfo in metadataInfoList)
        {
            try
            {
                res.Entities.Add(await ProcessMetadataAsync(metadataInfo.metadata, action, productId));
            }
            catch (Exception e)
            {
                //If metadata required not created - product deleted and process should stop
                if (action == ProductAdapterAction.Create && metadataInfo.required)
                    throw await StartCompensationAsync(productId, project.Code);

                if (e is WarningAssetProjectProcessor warning)
                    res.Warnings.Add(warning);
                else
                {
                    res.Errors.Add(new ErrorAssetProjectProcessor(e.Message, project.Code, EntitiesType.Metadata));
                }
            }
        }

        return res;
    }

    private async Task<AssetToDefectDojoMapper> ProcessMetadataAsync(Metadata metadata, ProductAdapterAction action, int?productId)
    {
        switch (action)
        {
            case ProductAdapterAction.Create:
                return await CreateMetadataAsync(metadata);
            case ProductAdapterAction.Update:
                metadata.Product=productId?? throw new Exception("Update metadata requested but no productId found or provided");
                return await UpdateMetadataAsync(metadata);
            case ProductAdapterAction.None:
            default:
                throw new Exception($"Invalid action requested {nameof(action)}");
        }
    }

    private async Task<ErrorAssetProjectProcessor> StartCompensationAsync(int productId, string code)
    {
        if (await defectDojoConnector.DeleteProductAsync(productId))
        {
            return new ErrorAssetProjectProcessor(
                $"Metadata with AssetCode could not be created; Compensation successful- Product with Id '{productId}' with code {code} has been deleted",
                code, EntitiesType.Metadata);
        }

        return new ErrorAssetProjectProcessor(
            $"Metadata with AssetCode could not be created; Compensation has failed - Product with Id '{productId}' with code {code} could not be deleted.PLease clean DefectDojo manually",
            code, EntitiesType.Product);
    }

    private async Task<AssetToDefectDojoMapper> CreateMetadataAsync(Metadata metadata)
    {
        var metadataRes = await defectDojoConnector.CreateMetadataAsync(metadata);
        return new AssetToMetadataMapper(metadataRes.Name, metadata.Id);
    }

    private async Task<AssetToDefectDojoMapper> UpdateMetadataAsync(Metadata metadata)
    {
        var originalMetadata = await GetOriginalMetadataAsync(metadata);
        if (originalMetadata == null)
            throw new Exception($"No metadata '{metadata.Name}' linked to product with Id {metadata.Product} found, update could not be processed");
        
        metadata.Id = originalMetadata.Id;
        var updateRes = await defectDojoConnector.UpdateMetadataAsync(metadata);

        return new AssetToMetadataMapper(metadata.Name, updateRes.Id);
    }

    private  async Task<Metadata?> GetOriginalMetadataAsync(Metadata metadata)
    {
        var searchParams = new Dictionary<string, string>
        {
            { "name", metadata.Name },
            { "product", metadata.Product.ToString() }
        };
        return await defectDojoConnector.GetMetadataAsync(searchParams);

    }
}