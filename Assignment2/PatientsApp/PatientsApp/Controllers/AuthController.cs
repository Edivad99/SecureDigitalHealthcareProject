using Microsoft.AspNetCore.Mvc;
using PatientsApp.Common.Models;

namespace PatientsApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public AuthController()
    {
    }

    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult SignUp(User user)
    {
        return StatusCode(StatusCodes.Status201Created);
    }
}

