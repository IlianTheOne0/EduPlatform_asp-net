using TestingPlatform.Constants;
using TestingPlatform.Data;
using TestingPlatform.Middlewares;

using TestingPlatform.Features.Users.Interfaces;
using TestingPlatform.Features.Users.Services;

using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Categories.Services;

using TestingPlatform.Features.Sessions.Interfaces;
using TestingPlatform.Features.Sessions.Services;

using TestingPlatform.Features.Student.Interfaces;
using TestingPlatform.Features.Student.Services;

using TestingPlatform.Features.Tests.Interfaces;
using TestingPlatform.Features.Tests.Services;

using TestingPlatform.Features.Users.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages
(
    options =>
    {
        options.RootDirectory = "/Features";
        options.Conventions.AddFolderRouteModelConvention
        (
            "/",
            model =>
            {
                if (model.RouteValues.TryGetValue("page", out var pageRouteValue))
                {
                    var currentPage = pageRouteValue?.ToString();
                    if (!string.IsNullOrEmpty(currentPage) && currentPage.Contains("/Pages/")) { model.RouteValues["page"] = currentPage.Replace("/Pages/", "/"); }
                }

                foreach (var selector in model.Selectors)
                {
                    var template = selector.AttributeRouteModel?.Template;
                    if (!string.IsNullOrEmpty(template) && template.Contains("Pages/", StringComparison.OrdinalIgnoreCase))
                    {
                        var cleanTemplate = template.Replace("Pages/", "", StringComparison.OrdinalIgnoreCase);
                        selector.AttributeRouteModel!.Template = cleanTemplate;
                    }
                }
            }
        );
    }
);

builder.Services.ConfigureApplicationCookie
(
    options =>
    {
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/Index";

        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    }
);

builder.Services.AddAuthorization
(
    options =>
    {
        options.AddPolicy(AppPolicies.AdminOnly, policy => policy.RequireRole(AppRoles.Administrator));
        options.AddPolicy(AppPolicies.TeacherOrAdmin, policy => policy.RequireRole(AppRoles.Teacher, AppRoles.Administrator));
        options.AddPolicy(AppPolicies.StudentOnly, policy => policy.RequireRole(AppRoles.Student));
    }
);

builder.Services.AddScoped<ITestAttemptService, TestAttemptService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ITestSessionService, TestSessionService>();
builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();

builder.Services.AddHostedService<AttemptTimeoutWorker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string[] roles = { AppRoles.Administrator, AppRoles.Teacher, AppRoles.Student };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role)) { await roleManager.CreateAsync(new IdentityRole(role)); }
    }

    string adminEmail = "admin@testing.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Admin"
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded) { await userManager.AddToRoleAsync(admin, AppRoles.Administrator); }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RootRedirectMiddleware>();
app.UseMiddleware<LoginRequiredMiddleware>();

app.MapRazorPages();
app.Run();