namespace TestingPlatform.Features.Users.Pages;

using TestingPlatform.Features.Users.Models;
using TestingPlatform.Features.Users.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    [BindProperty] public InputViewModel Input { get; set; } = new();

    public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager) { _userManager = userManager; _signInManager = signInManager; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

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
            await _userManager.AddToRoleAsync(user, "Student");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToPage("/Student/Dashboard");
        }
        foreach (var error in result.Errors) { ModelState.AddModelError(string.Empty, error.Description); }

        return Page();
    }
}