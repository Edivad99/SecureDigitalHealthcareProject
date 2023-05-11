using System.ComponentModel.DataAnnotations;

namespace PatientsApp.Client.Models;

public class AuthenticationUserModel
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Two Factor Authentication Code is required")]
    [StringLength(6, MinimumLength = 6)]
    public string TwoFACode { get; set; } = string.Empty;
}
