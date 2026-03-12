namespace TestingPlatform.Features.Tests.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = AppPolicies.TeacherOrAdmin)]
public class IndexModel : PageModel
{
    private readonly ITestService _testService;
    private readonly ICategoryService _categoryService;

    public List<Test> Tests { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
    [BindProperty(SupportsGet = true)] public int? CategoryId { get; set; }

    public IndexModel(ITestService testService, ICategoryService categoryService) { _testService = testService; _categoryService = categoryService; }

    public async Task OnGetAsync()
    {
        Categories = await _categoryService.GetAllCategoriesAsync();
        Tests = await _testService.GetTestsAsync(SearchTerm, CategoryId);
    }
}