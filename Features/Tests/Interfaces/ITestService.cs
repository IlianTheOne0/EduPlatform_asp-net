namespace TestingPlatform.Features.Tests.Interfaces;

using TestingPlatform.Features.Tests.Models;

public interface ITestService
{
    Task<List<Test>> GetTestsAsync(string? searchTerm, int? categoryId);
    Task<Test?> GetTestDetailsAsync(int testId, string currentUserId, bool isAdministrator);
    Task<Test> CreateTestAsync(Test test);
    Task<bool> UpdateTestAsync(Test test, string currentUserId, bool isAdministrator);
    Task<bool> DeleteTestAsync(int testId, string currentUserId, bool isAdministrator);
    Task<bool> AddQuestionAsync(int testId, Question question, string currentUserId, bool isAdministrator);
    Task<bool> RemoveQuestionAsync(int testId, int questionId, string currentUserId, bool isAdministrator);
    Task<bool> UpdateTestStatusAsync(int testId, TestStatus status, string currentUserId, bool isAdministrator);
}