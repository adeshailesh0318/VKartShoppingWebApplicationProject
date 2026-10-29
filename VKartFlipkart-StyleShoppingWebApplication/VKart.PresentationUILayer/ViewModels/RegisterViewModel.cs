using System.ComponentModel.DataAnnotations;

namespace VKart.PresentationLayer.ViewModels;

public class RegisterViewModel
{
    [Required, MaxLength(100), Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, MaxLength(15), Display(Name = "Mobile Number")]
    public string MobileNumber { get; set; } = string.Empty;

    [Required, MinLength(6), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match."),
     DataType(DataType.Password), Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}