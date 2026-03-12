namespace TestingPlatform.Features.Users.ViewModels;

using System.ComponentModel.DataAnnotations;

public class LoginInputModel
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}