namespace TestingPlatform.Features.Users.Pages;

using TestingPlatform.Features.Users.Models;
using TestingPlatform.Features.Users.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    [BindProperty] public InputViewModel Input { get; set; } = new();

    public ProfileModel(UserManager<User> userManager, SignInManager<User> signInManager) { _userManager = userManager; _signInManager = signInManager; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) { return NotFound(); }

        var roles = await _userManager.GetRolesAsync(user);

        Input.FirstName = user.FirstName;
        Input.LastName = user.LastName;
        Input.DateOfBirth = user.DateOfBirth;
        Input.Email = user.Email ?? string.Empty;
        Input.Role = roles.FirstOrDefault() ?? "Unknown";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) { return NotFound(); }

        if (!ModelState.IsValid)
        {
            var roles = await _userManager.GetRolesAsync(user);

            Input.Email = user.Email ?? string.Empty;
            Input.Role = roles.FirstOrDefault() ?? "Unknown";
            
            return Page();
        }

        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.DateOfBirth = Input.DateOfBirth.HasValue
            ? DateTime.SpecifyKind(Input.DateOfBirth.Value, DateTimeKind.Utc)
            : null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["SuccessMessage"] = "Profile updated successfully.";

        return RedirectToPage();
    }
}