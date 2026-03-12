namespace TestingPlatform.Features.Users.ViewModels;

using System.ComponentModel.DataAnnotations;

public class InputViewModel
{
    [Required, Display(Name = "First Name")] public string FirstName { get; set; } = string.Empty;
    [Required, Display(Name = "Last Name")] public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [DataType(DataType.Date), Display(Name = "Date of Birth")] public DateTime? DateOfBirth { get; set; }

    [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)][Compare("Password", ErrorMessage = "Passwords do not match.")] public string ConfirmPassword { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}