namespace TestingPlatform.Features.Sessions.Interfaces;

using TestingPlatform.Features.Student.ViewModels;
using TestingPlatform.Features.Sessions.Models;

public interface ITestAttemptService
{
    Task<TestAttempt?> GetAttemptWithQuestionsAsync(int attemptId, string userId);
    Task<TestAttempt?> GetAttemptResultAsync(int attemptId, string userId);
    Task<TestAttempt?> SubmitAttemptAsync(int attemptId, string userId, List<AttemptAnswerInput> answers);
    Task AutoCompleteAbandonedAttemptAsync(int attemptId);
    Task<bool> SaveSingleAnswerAsync(int attemptId, string userId, int questionId, int selectedOptionId);
}