namespace TestingPlatform.Features.Student.Services;

using TestingPlatform.Data;
using TestingPlatform.Features.Sessions.Models;
using TestingPlatform.Features.Student.Interfaces;

using Microsoft.EntityFrameworkCore;

public class StudentDashboardService : IStudentDashboardService
{
    private readonly AppDbContext _context;

    public StudentDashboardService(AppDbContext context) => _context = context;

    public async Task<List<TestAttempt>> GetStudentAttemptsAsync(string studentId, string? searchTerm, int? categoryId)
    {
        var query = _context.TestAttempts
            .Include(attempt => attempt.TestSession!)
            .ThenInclude(session => session.Test!)
            .ThenInclude(test => test.Category)
            .Include(attempt => attempt.TestSession!)
            .ThenInclude(session => session.Test!)
            .ThenInclude(test => test.Creator)
            .Where(attempt => attempt.UserId == studentId && attempt.CompletedAt != null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();

            query = query.Where
            (
                attempt =>
                    attempt.TestSession!.Test!.Title.ToLower().Contains(term) ||
                    attempt.TestSession!.Test!.Creator!.FirstName.ToLower().Contains(term) ||
                    attempt.TestSession!.Test!.Creator!.LastName.ToLower().Contains(term)
            );
        }

        if (categoryId.HasValue) { query = query.Where(attempt => attempt.TestSession!.Test!.CategoryId == categoryId.Value); }

        return await query.OrderByDescending(attempt => attempt.CompletedAt).ToListAsync();
    }
}