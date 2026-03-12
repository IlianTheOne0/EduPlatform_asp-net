namespace TestingPlatform.Features.Admin.Categories.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = AppPolicies.TeacherOrAdmin)]
public class IndexModel : PageModel
{
    private readonly ICategoryService _categoryService;

    public List<Category> Categories { get; set; } = new();

    public IndexModel(ICategoryService categoryService) => _categoryService = categoryService;

    public async Task OnGetAsync() => Categories = await _categoryService.GetAllCategoriesAsync();
}