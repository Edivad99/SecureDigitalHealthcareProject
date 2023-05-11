namespace PatientsApp.Data.Entity;

public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Key2FA { get; set; }
}
