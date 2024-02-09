﻿using System.ComponentModel;
using DefectDojoJob.Models;

namespace DefectDojoJob.Services;

public interface IEntitiesProcessingResult
{
    public List<AssetToDefectDojoMapper> Entities { get; set; }
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; }
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } 
    public EntityType EntityType { get; }
}