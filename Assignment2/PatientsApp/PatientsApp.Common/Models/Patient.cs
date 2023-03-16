namespace PatientsApp.Common.Models;

public class Patient
{
    public Guid Id { get; set; }

    public String FirstName { get; set; }

    public String LastName { get; set; }

    public String Email { get; set; }

    public String Password { get; set; }

    public String Gender { get; set; }

    public DateOnly Birthdate { get; set; }

    public String Address { get; set; }

    public String Phone { get; set; }

    public bool Terms { get; set; }
}
