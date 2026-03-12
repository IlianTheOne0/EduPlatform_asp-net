namespace TestingPlatform.Features.Admin.Users.ViewModels;

using System.ComponentModel.DataAnnotations;

public class InputViewModel
{
    [Required] public string Role { get; set; }
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
}

public class ExtendedInputViewModel : InputViewModel
{
    [Required] public string UserId { get; set; } = string.Empty;
}