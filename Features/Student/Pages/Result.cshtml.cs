namespace TestingPlatform.Features.Student.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Sessions.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.StudentOnly)]
public class ResultModel : PageModel
{
    private readonly ITestAttemptService _attemptService;

    public TestAttempt Attempt { get; set; } = default!;

    public ResultModel(ITestAttemptService attemptService) => _attemptService = attemptService;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var attempt = await _attemptService.GetAttemptResultAsync(id, userId);

        if (attempt == null || attempt.CompletedAt == null) { return RedirectToPage("Dashboard"); }
        Attempt = attempt;

        return Page();
    }
}