using Microsoft.AspNetCore.Mvc;
using PatientsApp.Common.Models;
using PatientsApp.Data.Repository;

namespace PatientsApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthRepository repository;
    private readonly ILogger<AuthController> logger;

    public AuthController(AuthRepository repository, ILogger<AuthController> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> SignUpAsync(User user)
    {
        try
        {
            logger.LogInformation($"New registration request");
            await repository.AddUserAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = user.Email,
                Password = user.Password
            });
            logger.LogInformation($"Registration completed");
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in SignUpAsync");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

