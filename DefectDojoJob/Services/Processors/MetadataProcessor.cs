using DefectDojoJob.Models.Adapters;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class MetadataProcessor:IMetadataProcessor
{
    private readonly IDefectDojoConnector defectDojoConnector;

    public MetadataProcessor(IDefectDojoConnector defectDojoConnector)
    {
        this.defectDojoConnector = defectDojoConnector;
    }
    public async Task<MetadataProcessingResult> ProcessProjectMetadata(AssetProject project)
    {
        throw new NotImplementedException();

    }

    private async Task<AssetToDefectDojoMapper> CreateMetadata(Metadata metadata)
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

public interface IMetadataProcessor
{
    public Task<MetadataProcessingResult> ProcessProjectMetadata(AssetProject project);
}