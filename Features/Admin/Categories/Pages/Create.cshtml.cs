namespace TestingPlatform.Features.Admin.Categories.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Categories.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Authorize(Policy = AppPolicies.AdminOnly)]
public class CreateModel : PageModel
{
    private readonly ICategoryService _categoryService;

    [BindProperty] public string Name { get; set; } = string.Empty;

    public CreateModel(ICategoryService categoryService) => _categoryService = categoryService;

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) { return Page(); }
        await _categoryService.CreateCategoryAsync(Name);
        
        return RedirectToPage("Index");
    }
}