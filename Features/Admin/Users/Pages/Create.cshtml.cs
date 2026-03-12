namespace TestingPlatform.Features.Admin.Users.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Admin.Users.ViewModels;
using TestingPlatform.Features.Users.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = AppPolicies.AdminOnly)]
public class CreateTeacherModel : PageModel
{
    private readonly UserManager<User> _userManager;

    [BindProperty] public InputViewModel Input { get; set; } = new();

    public CreateTeacherModel(UserManager<User> userManager) => _userManager = userManager;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) { return Page(); }

        var user = new User
        {
            UserName = Input.Email,
            Email = Input.Email,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            RegisteredAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, Input.Role);
            TempData["SuccessMessage"] = $"{Input.Role} account created successfully.";
            
            return RedirectToPage("/Admin/Index");
        }

        foreach (var error in result.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
        return Page();
    }
}