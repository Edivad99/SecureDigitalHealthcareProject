using System.ComponentModel.DataAnnotations;
using PatientsApp.Common.Attributes;

namespace PatientsApp.Common.Models;

public class User
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [PasswordStrength(3)]
    public string Password { get; set; }
}
