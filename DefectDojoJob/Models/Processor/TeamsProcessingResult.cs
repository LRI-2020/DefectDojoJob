﻿using DefectDojoJob.Models;

namespace DefectDojoJob.Services;

public class TeamsProcessingResult : IEntitiesProcessingResult
{
    public List<(string AssetIdentifier, int DefectDojoId)> Entities { get; set; } = new();
    public List<ErrorAssetProjectInfoProcessor> Errors { get; set; } = new();
    public List<WarningAssetProjectInfoProcessor> Warnings { get; set; } = new();
    public EntityType EntityType { get; } = EntityType.Team;
}