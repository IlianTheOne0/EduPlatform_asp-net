namespace TestingPlatform.Features.Tests.Services;

using TestingPlatform.Data;
using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.EntityFrameworkCore;

public class TestService : ITestService
{
    private readonly AppDbContext _context;

    public TestService(AppDbContext context) => _context = context;

    public async Task<List<Test>> GetTestsAsync(string? searchTerm, int? categoryId)
    {
        var query = _context.Tests
            .Include(test => test.Category)
            .Include(test => test.Creator)
            .Include(test => test.Questions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where
            (
                test =>
                    test.Title.ToLower().Contains(term) ||
                    test.Creator!.FirstName.ToLower().Contains(term) ||
                    test.Creator!.LastName.ToLower().Contains(term)
            );
        }

        if (categoryId.HasValue) { query = query.Where(test => test.CategoryId == categoryId.Value); }

        return await query.OrderByDescending(test => test.CreatedAt).ToListAsync();
    }

    public async Task<Test?> GetTestDetailsAsync(int testId, string currentUserId, bool isAdministrator)
    {
        var test = await _context.Tests
            .Include(test => test.Category)
            .Include(test => test.Sessions)
            .Include(test => test.Questions)
                .ThenInclude(question => question.AnswerOptions)
            .FirstOrDefaultAsync(test => test.Id == testId);

        if (test == null) { return null; }
        if (!isAdministrator && test.CreatorId != currentUserId) { return null; }

        return test;
    }

    public async Task<bool> AddQuestionAsync(int testId, Question question, string currentUserId, bool isAdministrator)
    {
        var test = await GetTestDetailsAsync(testId, currentUserId, isAdministrator);
        if (test == null || test.Status != TestStatus.Draft) { return false; }

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveQuestionAsync(int testId, int questionId, string currentUserId, bool isAdministrator)
    {
        var test = await GetTestDetailsAsync(testId, currentUserId, isAdministrator);
        if (test == null || test.Status != TestStatus.Draft) { return false; }

        var question = await _context.Questions.FindAsync(questionId);
        if (question != null && question.TestId == testId)
        {
            var answersToDelete = _context.AttemptAnswers.Where(answer => answer.QuestionId == questionId);

            _context.AttemptAnswers.RemoveRange(answersToDelete);

            _context.Questions.Remove(question);

            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateTestStatusAsync(int testId, TestStatus status, string currentUserId, bool isAdministrator)
    {
        var test = await GetTestDetailsAsync(testId, currentUserId, isAdministrator);
        if (test == null) { return false; }

        if (status == TestStatus.Published && test.Questions.Count == 0) { return false; }

        test.Status = status;
        test.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Test> CreateTestAsync(Test test)
    {
        _context.Tests.Add(test);
        await _context.SaveChangesAsync();
        
        return test;
    }

    public async Task<bool> UpdateTestAsync(Test updatedTest, string currentUserId, bool isAdministrator)
    {
        var test = await _context.Tests.FindAsync(updatedTest.Id);

        if (test == null || test.Status != TestStatus.Draft) { return false; }
        if (!isAdministrator && test.CreatorId != currentUserId) { return false; }

        test.Title = updatedTest.Title;
        test.Description = updatedTest.Description;
        test.CategoryId = updatedTest.CategoryId;
        test.TimeLimitMinutes = updatedTest.TimeLimitMinutes;
        test.ShowResultsAtEnd = updatedTest.ShowResultsAtEnd;
        test.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTestAsync(int testId, string currentUserId, bool isAdministrator)
    {
        var test = await _context.Tests.FindAsync(testId);

        if (test == null) { return false; }
        if (!isAdministrator && test.CreatorId != currentUserId) { return false; }

        var answersToDelete = _context.AttemptAnswers.Where(answer => answer.Question!.TestId == testId);
        _context.AttemptAnswers.RemoveRange(answersToDelete);

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();

        return true;
    }
}