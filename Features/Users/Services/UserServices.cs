namespace TestingPlatform.Features.Users.Services;

using TestingPlatform.Data;
using TestingPlatform.Features.Users.Interfaces;
using TestingPlatform.Features.Users.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public UserService(UserManager<User> userManager, AppDbContext context) { _userManager = userManager; _context = context; }

    public async Task<IdentityResult> DeleteUserAndCascadeDataAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) { return IdentityResult.Failed(new IdentityError { Description = "User not found" }); }

        var userAttempts = await _context.TestAttempts.Where(attempts => attempts.UserId == userId).ToListAsync();
        _context.TestAttempts.RemoveRange(userAttempts);

        var userTests = await _context.Tests.Where(tests => tests.CreatorId == userId).ToListAsync();
        if (userTests.Any())
        {
            var testIds = userTests.Select(tests => tests.Id).ToList();
            var answersToDelete = await _context.AttemptAnswers
                .Where(answers => testIds.Contains(answers.Question!.TestId))
                .ToListAsync();

            _context.AttemptAnswers.RemoveRange(answersToDelete);
            _context.Tests.RemoveRange(userTests);
        }

        await _context.SaveChangesAsync();
        return await _userManager.DeleteAsync(user);
    }
}