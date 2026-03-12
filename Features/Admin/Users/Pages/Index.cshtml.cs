namespace TestingPlatform.Features.Admin.Users.Pages;

using TestingPlatform.Constants;
using TestingPlatform.Features.Admin.Users.ViewModels;
using TestingPlatform.Features.Users.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

[Authorize(Policy = AppPolicies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly UserManager<User> _userManager;

    public List<UserViewModel> UsersList { get; set; } = new();

    public IndexModel(UserManager<User> userManager) => _userManager = userManager;

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users.OrderByDescending(user => user.RegisteredAt).ToListAsync();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            UsersList.Add
            (
                new UserViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? "No Email",
                    Role = roles.FirstOrDefault() ?? "None",
                    RegisteredAt = user.RegisteredAt
                }
            );
        }
    }
}