namespace TestingPlatform.Features.Sessions.Services;

using TestingPlatform.Data;
using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Sessions.Models;
using TestingPlatform.Features.Tests.Models;

using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

public class TestSessionService : ITestSessionService
{
    private readonly AppDbContext _context;

    public TestSessionService(AppDbContext context) => _context = context;

    public async Task<string?> LaunchSessionAsync(int testId, string teacherId, bool isAdministrator)
    {
        var test = await _context.Tests.FindAsync(testId);
        if (test == null || test.Status != TestStatus.Published) { return null; }
        if (!isAdministrator && test.CreatorId != teacherId) { return null; }

        string joinCode;
        do { joinCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString(); }
        while (await _context.TestSessions.AnyAsync(session => session.JoinCode == joinCode && session.IsActive));

        var session = new TestSession
        {
            TestId = testId,
            JoinCode = joinCode,
            TeacherId = teacherId
        };

        _context.TestSessions.Add(session);
        await _context.SaveChangesAsync();

        return joinCode;
    }

    public async Task<(bool Success, string ErrorMessage, int? AttemptId)> JoinSessionAsync(string joinCode, string studentId)
    {
        var session = await _context.TestSessions
            .Include(session => session.Test)
            .FirstOrDefaultAsync(session => session.JoinCode == joinCode && session.IsActive);

        if (session == null) { return (false, "Invalid or inactive PIN.", null); }

        var existing = await _context.TestAttempts.FirstOrDefaultAsync(attempt => attempt.TestSessionId == session.Id && attempt.UserId == studentId);
        if (existing != null)
        {
            if (existing.CompletedAt != null) { return (false, "You have already completed this test.", null); }
            return (true, string.Empty, existing.Id);
        }

        var attempt = new TestAttempt
        {
            TestSessionId = session.Id,
            UserId = studentId,
            StartedAt = DateTime.UtcNow
        };
        
        _context.TestAttempts.Add(attempt);
        await _context.SaveChangesAsync();

        return (true, string.Empty, attempt.Id);
    }
}