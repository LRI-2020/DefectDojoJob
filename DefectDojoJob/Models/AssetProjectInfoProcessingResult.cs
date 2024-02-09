namespace DefectDojoJob.Services;

public class AssetProjectInfoProcessingResult : IProcessingResult
{
    public int ProductId { get; set; }
    public TeamProcessingResult TeamProcessingResult { get; set; }
    public UserProcessingResult ApplicationOwnerProcessingResult { get; set; }
    public UserProcessingResult ApplicationOwnerBUProcessingResult { get; set; }
    public UserProcessingResult FunctionalOwnerProcessingResult { get; set; }
    public AssetProjectInfoProcessingAction Action { get; set; }
    public int EntityId { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool ProcessingSuccessful { get; set; }

}

public class UserProcessingResult: IProcessingResult
{
    public int EntityId { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool ProcessingSuccessful { get; set; }

}

public class TeamProcessingResult : IProcessingResult
{
    public int EntityId { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool ProcessingSuccessful { get; set; }

}

public interface IProcessingResult
{
    public int EntityId { get; set; }
    public List<string> Errors { get; set; }
    public List<string> Warnings { get; set; }
    public bool ProcessingSuccessful { get; set; }
}