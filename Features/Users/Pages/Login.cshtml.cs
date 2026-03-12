namespace TestingPlatform.Features.Users.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Users.Models;
using TestingPlatform.Features.Users.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    [BindProperty] public LoginInputModel Input { get; set; } = new();

    public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager) { _signInManager = signInManager; _userManager = userManager; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    switch (roles.FirstOrDefault())
                    {
                        case AppRoles.Administrator: { return RedirectToPage("/Admin/Index"); }
                        case AppRoles.Teacher: { return RedirectToPage("/Tests/Index"); }
                        case AppRoles.Student: { return RedirectToPage("/Student/Dashboard"); }
                    }
                }
                return RedirectToPage("/Home/Index");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return Page();
    }
}