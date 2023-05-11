using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PatientsApp.Common.Models;
using PatientsApp.Data.Repository;

namespace PatientsApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthRepository repository;
    private readonly IConfiguration configuration;
    private readonly ILogger<AuthController> logger;

    public AuthController(AuthRepository repository, IConfiguration configuration, ILogger<AuthController> logger)
    {
        this.repository = repository;
        this.configuration = configuration;
        this.logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("registration")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrationAsync(User user)
    {
        try
        {
            logger.LogInformation($"New registration request");
            var userDB = new Data.Entity.User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = user.Email,
                Password = user.Password
            };
            await repository.AddUserAsync(userDB);
            logger.LogInformation($"Registration completed");
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in SignUpAsync");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LogInAsync([FromForm(Name = "username"), Required] string username,
                                                [FromForm(Name = "password"), Required] string password)
    {
        var userDB = await repository.GetUserByEmailAsync(username);
        if (userDB is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (!BCrypt.Net.BCrypt.Verify(username + password, userDB.Password))
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        return StatusCode(StatusCodes.Status200OK, CreateToken(userDB));
    }

    private string CreateToken(Data.Entity.User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email)
        };

        var tokenCode = configuration.GetSection("AppSettings:Token").Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenCode));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}

