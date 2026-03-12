namespace TestingPlatform.Features.Tests.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Models;
using TestingPlatform.Features.Tests.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.TeacherOrAdmin)]
public class DetailsModel : PageModel
{
    private readonly ITestService _testService;
    private readonly ITestSessionService _sessionService;

    public Test Test { get; set; } = default!;

    [BindProperty] public QuestionInputViewModel NewQuestion { get; set; } = new();

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(AppRoles.Administrator);

    public DetailsModel(ITestService testService, ITestSessionService sessionService) { _testService = testService; _sessionService = sessionService; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var test = await _testService.GetTestDetailsAsync(id, CurrentUserId, IsAdmin);
        if (test == null) { return NotFound(); }
        Test = test;

        return Page();
    }

    public async Task<IActionResult> OnPostAddQuestionAsync(int id)
    {
        if (ModelState.IsValid)
        {
            var question = new Question
            {
                TestId = id,
                Text = NewQuestion.Text,
                Explanation = NewQuestion.Explanation,
                AnswerOptions = NewQuestion.Options.Select
                (
                    (text, i) =>
                    new AnswerOption
                    {
                        Text = text,
                        IsCorrect = i == NewQuestion.CorrectIndex
                    }
                ).ToList()
            };

            var success = await _testService.AddQuestionAsync(id, question, CurrentUserId, IsAdmin);
            if (!success) { return BadRequest("Cannot edit a published test or test not found."); }
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteQuestionAsync(int id, int questionId)
    {
        var success = await _testService.RemoveQuestionAsync(id, questionId, CurrentUserId, IsAdmin);
        if (!success) { return BadRequest(); }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostPublishAsync(int id)
    {
        var success = await _testService.UpdateTestStatusAsync(id, TestStatus.Published, CurrentUserId, IsAdmin);
        if (!success) { return BadRequest("Cannot publish empty test."); }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnpublishAsync(int id)
    {
        await _testService.UpdateTestStatusAsync(id, TestStatus.Draft, CurrentUserId, IsAdmin);

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostStartSessionAsync(int id)
    {
        var joinCode = await _sessionService.LaunchSessionAsync(id, CurrentUserId, IsAdmin);
        if (joinCode == null) { return BadRequest("Cannot start a session for a draft test."); }

        TempData["SuccessMessage"] = $"Session successfully generated! Students can join using PIN: {joinCode}";

        return RedirectToPage(new { id });
    }
}