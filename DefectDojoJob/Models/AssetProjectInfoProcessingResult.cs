﻿namespace DefectDojoJob.Services;

public class AssetProjectInfoProcessingResult
{
    public int ProductId { get; set; }
    public int TeamId { get; set; }
    public int AssetId { get; set; }
    public AssetProjectInfoProcessingAction Action { get; set; }
    public bool HasErrors { get; set; }
    public bool HasWarnings { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<int> UserIds { get; set; } = new();
}