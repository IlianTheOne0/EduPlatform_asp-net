namespace TestingPlatform.Features.Tests.ViewModels;

public class QuestionInputViewModel
{
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public List<string> Options { get; set; } = new string[4].ToList();
    public int CorrectIndex { get; set; }
}