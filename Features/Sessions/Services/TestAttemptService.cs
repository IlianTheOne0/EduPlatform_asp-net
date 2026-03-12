namespace TestingPlatform.Features.Sessions.Services;

using TestingPlatform.Data;
using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Sessions.Models;
using TestingPlatform.Features.Student.ViewModels;

using Microsoft.EntityFrameworkCore;

public class TestAttemptService : ITestAttemptService
{
    private readonly AppDbContext _context;

    public TestAttemptService(AppDbContext context) => _context = context;

    public async Task<TestAttempt?> SubmitAttemptAsync(int attemptId, string userId, List<AttemptAnswerInput> answers)
    {
        var attempt = await _context.TestAttempts
                .Include(attempt => attempt.TestSession!)
                .ThenInclude(session => session.Test)
                .FirstOrDefaultAsync(attempt => attempt.Id == attemptId && attempt.UserId == userId);

        if (attempt == null || attempt.CompletedAt != null) { return attempt; }

        var questions = await _context.Questions.Where(question => question.TestId == attempt.TestSession!.TestId).ToListAsync();
        int maxScore = questions.Sum(question => question.Points > 0 ? question.Points : 1);

        if (attempt.TestSession!.Test!.TimeLimitMinutes > 0)
        {
            var endTimeWithGracePeriod = attempt.StartedAt.AddMinutes(attempt.TestSession.Test.TimeLimitMinutes).AddMinutes(1);
            if (DateTime.UtcNow > endTimeWithGracePeriod) { answers.Clear(); }
        }

        foreach (var answer in answers)
        {
            if (!answer.SelectedOptionId.HasValue || answer.SelectedOptionId.Value <= 0) { continue; }

            var existingAnswer = await _context.AttemptAnswers
                .FirstOrDefaultAsync(ans => ans.TestAttemptId == attemptId && ans.QuestionId == answer.QuestionId);

            var option = await _context.AnswerOptions.Include(option => option.Question).FirstOrDefaultAsync(option => option.Id == answer.SelectedOptionId);
            bool isCorrect = option?.IsCorrect ?? false;

            if (existingAnswer != null)
            {
                existingAnswer.SelectedOptionId = answer.SelectedOptionId;
                existingAnswer.IsCorrect = isCorrect;
            }
            else
            {
                _context.AttemptAnswers.Add
                (
                    new AttemptAnswer
                    {
                        TestAttemptId = attemptId,
                        QuestionId = answer.QuestionId,
                        SelectedOptionId = answer.SelectedOptionId,
                        IsCorrect = isCorrect
                    }
                );
            }
        }

        await _context.SaveChangesAsync();

        int score = await _context.AttemptAnswers
            .Where(attempt => attempt.TestAttemptId == attemptId && attempt.IsCorrect)
            .SumAsync(attempt => attempt.Question!.Points > 0 ? attempt.Question!.Points : 1);

        attempt.CompletedAt = DateTime.UtcNow;
        attempt.TotalScore = score;
        attempt.MaxPossibleScore = maxScore;

        await _context.SaveChangesAsync();

        return attempt;
    }

    public async Task AutoCompleteAbandonedAttemptAsync(int attemptId)
    {
        var attempt = await _context.TestAttempts
            .Include(attempt => attempt.TestSession!)
            .ThenInclude(session => session.Test)
            .FirstOrDefaultAsync(attempt => attempt.Id == attemptId);

        if (attempt == null || attempt.CompletedAt != null) { return; }

        var questions = await _context.Questions.Where(question => question.TestId == attempt.TestSession!.TestId).ToListAsync();
        int maxScore = questions.Sum(question => question.Points > 0 ? question.Points : 1);

        attempt.TotalScore = 0;
        attempt.MaxPossibleScore = maxScore;
        attempt.CompletedAt = attempt.StartedAt.AddMinutes(attempt.TestSession!.Test!.TimeLimitMinutes);

        await _context.SaveChangesAsync();
    }

    public async Task<TestAttempt?> GetAttemptWithQuestionsAsync(int attemptId, string userId) =>
        await _context.TestAttempts
            .Include(attempt => attempt.TestSession!)
            .ThenInclude(session => session.Test!)
            .ThenInclude(test => test.Questions)
            .ThenInclude(question => question.AnswerOptions)
            .FirstOrDefaultAsync(attempt => attempt.Id == attemptId && attempt.UserId == userId);

    public async Task<TestAttempt?> GetAttemptResultAsync(int attemptId, string userId) => 
        await _context.TestAttempts
            .Include(attempt => attempt.TestSession!)
            .ThenInclude(session => session.Test)
            .Include(attempt => attempt.Answers)
            .ThenInclude(answer => answer.Question!)
            .ThenInclude(question => question.AnswerOptions)
            .Include(attempt => attempt.Answers)
            .ThenInclude(answer => answer.SelectedOption)
            .FirstOrDefaultAsync(attempt => attempt.Id == attemptId && attempt.UserId == userId);

    public async Task<bool> SaveSingleAnswerAsync(int attemptId, string userId, int questionId, int selectedOptionId)
    {
        var attempt = await _context.TestAttempts
            .Include(attempt => attempt.TestSession!)
            .ThenInclude(session => session.Test)
            .FirstOrDefaultAsync(attempt => attempt.Id == attemptId && attempt.UserId == userId);

        if (attempt == null || attempt.CompletedAt != null) { return false; }

        if (attempt.TestSession!.Test!.TimeLimitMinutes > 0)
        {
            var expiration = attempt.StartedAt.AddMinutes(attempt.TestSession.Test.TimeLimitMinutes).AddMinutes(1);
            if (DateTime.UtcNow > expiration) { return false; }
        }

        var existingAnswer = await _context.AttemptAnswers
            .FirstOrDefaultAsync(attempt => attempt.TestAttemptId == attemptId && attempt.QuestionId == questionId);

        var option = await _context.AnswerOptions.FindAsync(selectedOptionId);
        bool isCorrect = option?.IsCorrect ?? false;

        if (existingAnswer != null)
        {
            existingAnswer.SelectedOptionId = selectedOptionId;
            existingAnswer.IsCorrect = isCorrect;
        }
        else
        {
            _context.AttemptAnswers.Add
            (
                new AttemptAnswer
                {
                    TestAttemptId = attemptId,
                    QuestionId = questionId,
                    SelectedOptionId = selectedOptionId,
                    IsCorrect = isCorrect
                }
            );
        }

        await _context.SaveChangesAsync();

        return true;
    }
}