﻿using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor.Errors;

namespace DefectDojoJob.Models.Processor.Results;

public class UsersProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; } = new();
    public List<ErrorAssetProjectProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectProcessor> Warnings { get; set; } = new();
    public EntitiesType EntitiesType { get; } = EntitiesType.User;
}