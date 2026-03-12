namespace TestingPlatform.Features.Sessions.Interfaces;

public interface ITestSessionService
{
    Task<string?> LaunchSessionAsync(int testId, string teacherId, bool isAdministrator);
    Task<(bool Success, string ErrorMessage, int? AttemptId)> JoinSessionAsync(string joinCode, string studentId);
}