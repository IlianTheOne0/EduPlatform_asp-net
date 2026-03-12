namespace TestingPlatform.Features.Admin.Pages;

using TestingPlatform.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = AppPolicies.AdminOnly)]
public class IndexModel : PageModel
{
    public void OnGet() { }
}