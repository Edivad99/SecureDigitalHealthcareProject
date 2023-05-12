﻿using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PatientsApp.Common.DTO;
using PatientsApp.Common.Models;
using PatientsApp.Data.Repository;

namespace PatientsApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private static readonly TimeSpan timeTolerance = TimeSpan.FromSeconds(30);

    private readonly AuthRepository repository;
    private readonly TwoFactorAuthenticator twoFactAuth;
    private readonly IConfiguration configuration;

    public AuthController(AuthRepository repository, TwoFactorAuthenticator twoFactAuth, IConfiguration configuration)
    {
        this.repository = repository;
        this.twoFactAuth = twoFactAuth;
        this.configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("registration")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrationAsync(User user)
    {
        try
        {
            string key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8)); // 12 characters
            var userDB = new Data.Entity.User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = user.Email,
                Password = user.Password,
                Key2FA = key
            };
            await repository.AddUserAsync(userDB);

            var setupInfo = twoFactAuth.GenerateSetupCode("PatientsApp", userDB.Email, key, false, 3);

            return StatusCode(StatusCodes.Status201Created, new GoogleAuthDTO
            {
                QrCodeImageUrl = setupInfo.QrCodeSetupImageUrl,
                ManualEntrySetupCode = setupInfo.ManualEntryKey
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LogInAsync([FromForm(Name = "username"), Required] string username,
                                                [FromForm(Name = "password"), Required] string password/*,
                                                [FromForm(Name = "twofa_code"), Required] string twofa_code*/)
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

        /*if (!twoFactAuth.ValidateTwoFactorPIN(userDB.Key2FA, twofa_code, timeTolerance))
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }*/

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

