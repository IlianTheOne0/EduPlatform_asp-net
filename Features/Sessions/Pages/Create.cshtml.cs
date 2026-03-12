namespace TestingPlatform.Features.Sessions.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.TeacherOrAdmin)]
public class CreateModel : PageModel
{
    private readonly ITestService _testService;
    private readonly ITestSessionService _sessionService;

    [BindProperty(SupportsGet = true)] public int TestId { get; set; }
    public Test Test { get; set; } = default!;

    public CreateModel(ITestService testService, ITestSessionService sessionService) { _testService = testService; _sessionService = sessionService; }

    public async Task<IActionResult> OnGetAsync()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var test = await _testService.GetTestDetailsAsync(TestId, currentUserId, User.IsInRole("Administrator"));

        if (test == null || test.Status != TestStatus.Published) { return NotFound(); }
        Test = test;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var joinCode = await _sessionService.LaunchSessionAsync(TestId, currentUserId, User.IsInRole("Administrator"));

        if (joinCode == null) { return BadRequest("Could not launch session."); }

        TempData["NewSessionCode"] = joinCode;

        return RedirectToPage("/Tests/Index");
    }
}