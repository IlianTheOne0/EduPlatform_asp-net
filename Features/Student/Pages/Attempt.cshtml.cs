namespace TestingPlatform.Features.Student.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Sessions.Models;
using TestingPlatform.Features.Student.ViewModels;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.StudentOnly)]
public class AttemptModel : PageModel
{
    private readonly ITestAttemptService _attemptService;

    [BindProperty] public TestAttempt Attempt { get; set; } = default!;
    [BindProperty] public List<AttemptAnswerInput> Answers { get; set; } = new();

    public List<Question> Questions { get; set; } = new();
    
    public AttemptModel(ITestAttemptService attemptService) => _attemptService = attemptService;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var attempt = await _attemptService.GetAttemptWithQuestionsAsync(id, userId);

        if (attempt == null || attempt.CompletedAt != null) { return RedirectToPage("Dashboard"); }

        Attempt = attempt;
        Questions = attempt.TestSession?.Test?.Questions?.ToList() ?? new List<Question>();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var attempt = await _attemptService.SubmitAttemptAsync(id, userId, Answers);
        if (attempt == null) { return RedirectToPage("Dashboard"); }

        return RedirectToPage("Result", new { id = attempt.Id });
    }

    public async Task<IActionResult> OnPostAutoSaveAsync(int id, [FromBody] AutoSaveRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var success = await _attemptService.SaveSingleAnswerAsync(id, userId, request.QuestionId, request.SelectedOptionId);

        return new JsonResult(new { success });
    }
}