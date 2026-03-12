namespace TestingPlatform.Features.Sessions.Models;

using TestingPlatform.Features.Tests.Models;

public class AttemptAnswer
{
    public int Id { get; set; }
    
    public int TestAttemptId { get; set; }
    public TestAttempt? TestAttempt { get; set; }

    public int QuestionId { get; set; }
    public Question? Question { get; set; }

    public int? SelectedOptionId { get; set; }
    public AnswerOption? SelectedOption { get; set; }

    public bool IsCorrect { get; set; }
}