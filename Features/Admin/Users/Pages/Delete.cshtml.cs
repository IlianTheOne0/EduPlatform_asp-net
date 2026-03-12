namespace TestingPlatform.Features.Admin.Users.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Admin.Users.ViewModels;
using TestingPlatform.Features.Users.Interfaces;
using TestingPlatform.Features.Users.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = AppPolicies.AdminOnly)]
public class DeleteModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;

    [BindProperty] public ExtendedInputViewModel Input { get; set; } = new();

    public DeleteModel(UserManager<User> userManager, IUserService userService) { _userManager = userManager; _userService = userService; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) { return NotFound(); }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user.Id == currentUserId)
        {
            TempData["Error"] = "You cannot delete your own active session account.";
            return RedirectToPage("Index");
        }

        Input.UserId = user.Id;
        Input.Email = user.Email ?? "Unknown";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var result = await _userService.DeleteUserAndCascadeDataAsync(id);

        if (result.Succeeded) { TempData["SuccessMessage"] = "User account and all generated data were successfully deleted."; }
        else { TempData["Error"] = "An error occurred while deleting the user."; }

        return RedirectToPage("Index");
    }
}