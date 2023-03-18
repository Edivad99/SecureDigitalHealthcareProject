using System.ComponentModel.DataAnnotations;

namespace PatientsApp.Common.Models;

public class Patient
{
    public Guid Id { get; set; }

    [Required]
    public String FirstName { get; set; }
    [Required]
    public String LastName { get; set; }
    [Required]
    public String Email { get; set; }
    [Required]
    [MinLength(8)]
    public String Password { get; set; }
    [Required]
    public String Gender { get; set; }
    [Required]
    public DateOnly Birthdate { get; set; }
    [Required]
    public String Address { get; set; }
    [Required]
    public String Phone { get; set; }
    [Required]
    public bool Terms { get; set; }
}
