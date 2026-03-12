namespace TestingPlatform.Features.Tests.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.TeacherOrAdmin)]
public class DeleteModel : PageModel
{
    private readonly ITestService _testService;

    [BindProperty] public Test Test { get; set; } = default!;

    public DeleteModel(ITestService testService) => _testService = testService;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var test = await _testService.GetTestDetailsAsync(id, currentUserId, User.IsInRole("Administrator"));

        if (test == null) { return NotFound(); }
        Test = test;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var success = await _testService.DeleteTestAsync(id, currentUserId, User.IsInRole("Administrator"));

        if (!success) { return NotFound(); }

        return RedirectToPage("Index");
    }
}