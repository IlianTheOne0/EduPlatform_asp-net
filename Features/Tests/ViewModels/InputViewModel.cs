namespace TestingPlatform.Features.Tests.ViewModels;

public class InputViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int TimeLimitMinutes { get; set; } = 0;
    public bool ShowResultsAtEnd { get; set; } = true;
}