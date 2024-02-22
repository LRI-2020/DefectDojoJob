using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Extractors;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class MetadataProcessor:IMetadataProcessor
{
    private readonly IDefectDojoConnector defectDojoConnector;
    private readonly IMetadataExtractor metadataExtractor;

    public MetadataProcessor(IDefectDojoConnector defectDojoConnector, IMetadataExtractor metadataExtractor)
    {
        this.defectDojoConnector = defectDojoConnector;
        this.metadataExtractor = metadataExtractor;
    }
    public async Task<MetadataProcessingResult> ProcessProjectMetadataAsync(AssetProject project, ProductAdapterAction action,int productId)
    {
        
        var metadatas = metadataExtractor.ExtractMetadata(project, productId);
        foreach (var metadata in metadatas)
        {
            
        }


    }

    private async Task<AssetToDefectDojoMapper> CreateMetadata(Metadata metadata, bool required)
    {        
        //!!!! if Metadata Code --> compensation is error here
        throw new NotImplementedException();
        // if (await defectDojoConnector.DeleteProductAsync(createRes.Id))
        // {
        //     res.Errors.Add(new ErrorAssetProjectProcessor($"Metadata with AssetCode could not be created; Compensation successful- Product with Id '{createRes.Id}' with code {pi.Code} has been deleted",
        //         pi.Code,EntitiesType.Product));
        // }
        // res.Errors.Add(new ErrorAssetProjectProcessor($"Metadata with AssetCode could not be created; Compensation has failed - Product with Id '{createRes.Id}' with code {pi.Code} could not be deleted.PLease clean DefectDojo manually",
        //     pi.Code,EntitiesType.Product));

         var metadataRes =  await defectDojoConnector.CreateMetadataAsync(metadata);
         return new AssetToMetadataMapper(metadataRes.Name, metadata.Id);
    }    
    
    private async Task<AssetToDefectDojoMapper> Update(Metadata metadata)
    {
        throw new NotImplementedException();

    }
    
}