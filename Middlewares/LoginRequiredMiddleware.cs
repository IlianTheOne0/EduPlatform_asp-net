namespace TestingPlatform.Middlewares;

using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

public class LoginRequiredMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _loginPath;

    public LoginRequiredMiddleware(RequestDelegate next, IOptions<CookieAuthenticationOptions> cookieOptions)
    {
        _next = next;
        _loginPath = cookieOptions.Value.LoginPath.Value ?? "/Users/Login";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.TrimEnd('/').ToLowerInvariant() ?? "";
        var isStatic = path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib");
        var isAllowed = path == "" || path == "/home" || path == "/home/index" || path == "/users/login" || path == "/users/register";

        if (!isStatic && !isAllowed && (!context.User.Identity?.IsAuthenticated ?? true))
        {
            context.Response.Redirect(_loginPath);
            return;
        }

        await _next(context);
    }
}