using System.ComponentModel.DataAnnotations;

namespace PatientsApp.Common.Models;

public class Patient
{
    public Guid Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
    [Required]
    [RegularExpression("Male|Female|Other")]
    public string Gender { get; set; }
    [Required]
    public DateOnly Birthdate { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    [Phone]
    public string Phone { get; set; }
    [Required]
    public bool Terms { get; set; }
}
