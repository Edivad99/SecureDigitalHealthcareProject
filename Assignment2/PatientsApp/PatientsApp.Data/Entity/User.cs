namespace PatientsApp.Data.Entity;

public class User
{
    public readonly string Role = "doctor";
    public string Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Key2FA { get; set; }
}
