namespace TestingPlatform.Features.Admin.Users.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Admin.Users.ViewModels;
using TestingPlatform.Features.Users.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.AdminOnly)]
public class EditModel : PageModel
{
    private readonly UserManager<User> _userManager;

    [BindProperty] public ExtendedInputViewModel Input { get; set; } = new();

    public EditModel(UserManager<User> userManager) => _userManager = userManager;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) { return NotFound(); }

        var roles = await _userManager.GetRolesAsync(user);

        Input.UserId = user.Id;
        Input.FirstName = user.FirstName;
        Input.LastName = user.LastName;
        Input.Email = user.Email ?? string.Empty;
        Input.Role = roles.FirstOrDefault() ?? "Student";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) { return Page(); }

        var user = await _userManager.FindByIdAsync(Input.UserId);
        if (user == null) { return NotFound(); }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user.Id == currentUserId && Input.Role != "Administrator")
        {
            TempData["Error"] = "You cannot remove your own Administrator privileges.";
            return RedirectToPage("Index");
        }

        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.Email = Input.Email;
        user.UserName = Input.Email;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
            return Page();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(Input.Role))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, Input.Role);
        }

        TempData["SuccessMessage"] = $"Account for {user.FirstName} updated successfully.";
        return RedirectToPage("Index");
    }
}