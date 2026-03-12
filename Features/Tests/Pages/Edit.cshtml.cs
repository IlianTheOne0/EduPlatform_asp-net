namespace TestingPlatform.Features.Tests.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.TeacherOrAdmin)]
public class EditModel : PageModel
{
    private readonly ITestService _testService;
    private readonly ICategoryService _categoryService;

    [BindProperty] public Test Test { get; set; } = default!;
    public SelectList CategoryOptions { get; set; } = default!;

    public EditModel(ITestService testService, ICategoryService categoryService) { _testService = testService; _categoryService = categoryService; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole(AppRoles.Administrator);

        var test = await _testService.GetTestDetailsAsync(id, currentUserId, isAdmin);
        if (test == null) { return NotFound(); }

        if (test.Status != TestStatus.Draft)
        {
            TempData["Error"] = "Published tests cannot be edited. Revert to draft first.";
            return RedirectToPage("Details", new { id });
        }

        Test = test;
        var categories = await _categoryService.GetAllCategoriesAsync();
        CategoryOptions = new SelectList(categories, "Id", "Name");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole(AppRoles.Administrator);

        var success = await _testService.UpdateTestAsync(Test, currentUserId, isAdmin);
        if (!success) { return BadRequest("Could not update test. It may be published or you lack permissions."); }

        return RedirectToPage("Details", new { id = Test.Id });
    }
}