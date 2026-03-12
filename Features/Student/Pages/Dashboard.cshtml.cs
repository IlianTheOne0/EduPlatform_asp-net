namespace TestingPlatform.Features.Student.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Sessions.Models;
using TestingPlatform.Features.Student.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.StudentOnly)]
public class DashboardModel : PageModel
{
    private readonly IStudentDashboardService _dashboardService;
    private readonly ITestSessionService _sessionService;
    private readonly ICategoryService _categoryService;

    [BindProperty] public string JoinCode { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
    [BindProperty(SupportsGet = true)] public int? CategoryId { get; set; }
    
    public List<TestAttempt> Attempts { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    public string PlaceholderText => "\u2022 \u2022 \u2022 \u2022 \u2022 \u2022";

    public DashboardModel(IStudentDashboardService dashboardService, ITestSessionService sessionService, ICategoryService categoryService)
    {
        _dashboardService = dashboardService;
        _sessionService = sessionService;
        _categoryService = categoryService;
    }

    public async Task OnGetAsync()
    {
        Categories = await _categoryService.GetAllCategoriesAsync();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        Attempts = await _dashboardService.GetStudentAttemptsAsync(userId, SearchTerm, CategoryId);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var (success, errorMsg, attemptId) = await _sessionService.JoinSessionAsync(JoinCode, userId);

        if (!success) { TempData["Error"] = errorMsg; return RedirectToPage(); }

        return RedirectToPage("Attempt", new { id = attemptId });
    }
}