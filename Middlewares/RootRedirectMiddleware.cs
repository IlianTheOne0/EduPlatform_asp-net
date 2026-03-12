namespace TestingPlatform.Middlewares;

public class RootRedirectMiddleware
{
    private readonly RequestDelegate _next;

    public RootRedirectMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/")
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                if (context.User.IsInRole("Student")) { context.Response.Redirect("/Student/Dashboard"); }
                else { context.Response.Redirect("/Tests/Index"); }
            }
            else { context.Response.Redirect("/Home/Index"); }

            return;
        }

        await _next(context);
    }
}