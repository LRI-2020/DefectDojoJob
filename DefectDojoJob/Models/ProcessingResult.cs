namespace DefectDojoJob.Services;

public class ProcessingResult
{
    public List<TeamEntityProcessingResult> TeamsResult { get; set; } = new();
    public List<UserEntityProcessingResult> UsersResult { get; set; } = new();
}