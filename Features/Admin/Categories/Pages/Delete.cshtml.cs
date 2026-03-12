namespace TestingPlatform.Features.Admin.Categories.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = AppPolicies.AdminOnly)]
public class DeleteModel : PageModel
{
    private readonly ICategoryService _categoryService;

    [BindProperty] public Category Category { get; set; } = default!;

    public DeleteModel(ICategoryService categoryService) => _categoryService = categoryService;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) { return NotFound(); }
        Category = category;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var success = await _categoryService.DeleteCategoryAsync(id);
        if (!success) { return NotFound(); }

        TempData["SuccessMessage"] = "Category and associated data deleted.";
        return RedirectToPage("Index");
    }
}