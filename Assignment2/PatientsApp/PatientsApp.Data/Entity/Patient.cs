namespace PatientsApp.Data.Entity;

public class Patient
{
    public readonly string Role = "patient";
    public string Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public DateTime Birthdate { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public bool Terms { get; set; }
    public string ProfilePicture { get; set; }
}
