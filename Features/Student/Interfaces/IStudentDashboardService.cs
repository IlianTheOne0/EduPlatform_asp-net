namespace TestingPlatform.Features.Student.Interfaces;

using TestingPlatform.Features.Sessions.Models;

public interface IStudentDashboardService
{
    Task<List<TestAttempt>> GetStudentAttemptsAsync(string studentId, string? searchTerm, int? categoryId);
}