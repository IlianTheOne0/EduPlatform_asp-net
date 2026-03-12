namespace TestingPlatform.Features.Users.Interfaces;

using Microsoft.AspNetCore.Identity;

public interface IUserService
{
    Task<IdentityResult> DeleteUserAndCascadeDataAsync(string userId);
}