namespace TestingPlatform.Features.Tests.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Models;
using TestingPlatform.Features.Tests.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.TeacherOrAdmin)]
public class CreateModel : PageModel
{
    private readonly ITestService _testService;
    private readonly ICategoryService _categoryService;

    [BindProperty] public InputViewModel Input { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    public CreateModel(ITestService testService, ICategoryService categoryService) { _testService = testService; _categoryService = categoryService; }

    public async Task OnGetAsync() => Categories = await _categoryService.GetAllCategoriesAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Categories = await _categoryService.GetAllCategoriesAsync();
            return Page();
        }

        var test = new Test
        {
            Title = Input.Title,
            Description = Input.Description ?? "",
            CategoryId = Input.CategoryId,
            Status = TestStatus.Draft,
            CreatorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            TimeLimitMinutes = Input.TimeLimitMinutes,
            ShowResultsAtEnd = Input.ShowResultsAtEnd
        };

        var createdTest = await _testService.CreateTestAsync(test);
        return RedirectToPage("Details", new { id = createdTest.Id });
    }
}