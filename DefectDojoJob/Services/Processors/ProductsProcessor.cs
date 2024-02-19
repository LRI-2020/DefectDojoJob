﻿using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Models.Processor.Results;
using DefectDojoJob.Services.Interfaces;

namespace DefectDojoJob.Services.Processors;

public class ProductsProcessor : IProductsProcessor
{
    private readonly IConfiguration configuration;
    private readonly IDefectDojoConnector defectDojoConnector;
    private const string DefaultDescription = "Enter a description";

    public ProductsProcessor(IConfiguration configuration, IDefectDojoConnector defectDojoConnector)
    {
        this.configuration = configuration;
        this.defectDojoConnector = defectDojoConnector;
    }

    public async Task<ProductsProcessingResult> ProcessProductsAsync(List<AssetProjectInfo> projects,
        List<AssetToDefectDojoMapper> users)
    {
        var result = new ProductsProcessingResult();
        foreach (var project in projects)
        {
            try
            {
                var action = await ExistingProjectAsync(project) ? AssetProjectInfoProcessingAction.Update : AssetProjectInfoProcessingAction.Create;
                result.Entities.Add(await ProcessProductAsync(project, users, action));
            }
            catch (Exception e)
            {
                if (e is WarningAssetProjectInfoProcessor warning) result.Warnings.Add(warning);
                else
                {
                    result.Errors.Add(new ErrorAssetProjectInfoProcessor(e.Message, project.Code, EntityType.Product));
                }
            }
        }

        return result;
    }

    private async Task<bool> ExistingProjectAsync(AssetProjectInfo project)
    {
        var searchParams = new Dictionary<string, string>
        {
            { "name", "AssetCode" },
            { "value", project.Code }
        };

        var metadataTask = defectDojoConnector.GetMetadataAsync(searchParams);
        var productTask = defectDojoConnector.GetProductByNameAsync(project.Name);

        var metadata = await metadataTask;
        var product = await productTask;
        ValidateResults(metadata, product, project.Code, project.Name);

        return metadata != null && product != null;
    }

    private static void ValidateResults(Metadata? metadata, Product? product, string code, string name)
    {
        if (metadata == null && product != null)
            throw new ErrorAssetProjectInfoProcessor($" Product '{name}' has been found but no code '{code}' linked to it",
                code, EntityType.Product);
        if (metadata != null && product == null)
            throw new ErrorAssetProjectInfoProcessor($" Code '{code}' has been found but no product '{name}' linked to it",
                code, EntityType.Product);
        if (metadata?.Product != product?.Id)
            throw new ErrorAssetProjectInfoProcessor($" Code {code} and product '{name}' have been found but are not linked together in defect dojo",
                code, EntityType.Product);
    }

    public async Task<AssetToDefectDojoMapper> ProcessProductAsync(AssetProjectInfo projectInfo,
        List<AssetToDefectDojoMapper> users, AssetProjectInfoProcessingAction requiredAction)
    {
        var productType = await GetProductTypeAsync(projectInfo.ProductType, projectInfo.Code);

        var product = new Product(projectInfo.Name, SetDescription(projectInfo))
        {
            ProductTypeId = productType,
            Lifecycle = GetLifeCycle(projectInfo.State),
            TechnicalContact = GetUser(projectInfo, nameof(projectInfo.ApplicationOwner), users),
            TeamManager = GetUser(projectInfo, nameof(projectInfo.ApplicationOwnerBackUp), users),
            ProductManager = GetUser(projectInfo, nameof(projectInfo.FunctionalOwner), users),
            UserRecords = projectInfo.NumberOfUsers,
            ExternalAudience = projectInfo.OpenToPartner ?? false
        };

        var res = await defectDojoConnector.CreateProductAsync(product);

        return new AssetToDefectDojoMapper(projectInfo.Code, res.Id);
    }

    private static string SetDescription(AssetProjectInfo projectInfo)
    {
        return string.IsNullOrEmpty(projectInfo.ShortDescription?.Trim()) && string.IsNullOrEmpty(projectInfo.DetailedDescription?.Trim())
            ? DefaultDescription
            : $"Short Description : {projectInfo.ShortDescription ?? "/"}; Detailed Description : {projectInfo.DetailedDescription ?? "/"} ; ";
    }

    private async Task<int> GetProductTypeAsync(string? providedProductType, string assetIdentifier)
    {
        var defaultType = configuration["ProductType"];

        if (string.IsNullOrEmpty(providedProductType) && string.IsNullOrEmpty(defaultType))
            throw new ErrorAssetProjectInfoProcessor(
                "no product type provided and none found in the configuration file", assetIdentifier, EntityType.Product);

        ProductType? res;
        if (!string.IsNullOrEmpty(providedProductType)) res = await defectDojoConnector.GetProductTypeByNameAsync(providedProductType);
        else res = await defectDojoConnector.GetProductTypeByNameAsync(defaultType!);

        return res?.Id ??
               throw new ErrorAssetProjectInfoProcessor(
                   $"No product type was found, neither with the provided type nor with the default type", 
                   assetIdentifier, EntityType.Product);
    }

    private static Lifecycle? GetLifeCycle(string? state)
    {
        if (string.IsNullOrEmpty(state)) return null;
        switch (state.Trim())
        {
            case "EnConstruction": return Lifecycle.construction;
            case "EnService":
            case "EnCoursDeDeclassement":
                return Lifecycle.production;
            case "Declassee": return Lifecycle.retirement;
            default: return null;
        }
    }

    private static int? GetUser(AssetProjectInfo pi, string propertyName, List<AssetToDefectDojoMapper> users)
    {
        return users
            .Find(u => u.AssetIdentifier == (string)(pi.GetType().GetProperty(propertyName)?.GetValue(pi) ?? ""))
            ?.DefectDojoId;
    }
}