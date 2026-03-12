namespace TestingPlatform.Features.Tests.Models;

public class Question
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public QuestionType Type { get; set; } = QuestionType.SingleChoice;
    public int Points { get; set; } = 1;

    public int TestId { get; set; }
    public Test? Test { get; set; }

    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}

public class AnswerOption
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}