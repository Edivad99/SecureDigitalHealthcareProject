using PatientsApp.Common.Models;

namespace PatientsApp.Common.DTO;

public class PatientDTO : Patient
{
    public Guid Id { get; set; }
    public string ProfilePicture { get; set; }
}
