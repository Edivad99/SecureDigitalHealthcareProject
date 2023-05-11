using PatientsApp.Client.Models;

namespace PatientsApp.Client.Authentication;

public interface IAuthenticationService
{
    Task LoginAsync(AuthenticationUserModel user);
    Task LogoutAsync();
}