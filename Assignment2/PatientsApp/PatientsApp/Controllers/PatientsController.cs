using Microsoft.AspNetCore.Mvc;
using PatientsApp.Common.DTO;
using PatientsApp.Common.Models;
using PatientsApp.Data.Repository;

namespace PatientsApp.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientRepository repository;

    public PatientsController(PatientRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientsAsync()
    {
        var patients = await repository.GetPatientsAsync();
        return StatusCode(StatusCodes.Status200OK, patients.Select(MapToDTO));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        var patient = await repository.GetPatientAsync(id);
        if (patient is null)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, MapTo(patient));
    }

    [HttpGet("search/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPatientsByName(string name)
    {
        var patients = await repository.SearchPatientsByNameAsync(name);
        if (!patients.Any())
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, patients.Select(MapToDTO));
    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddPatientAsync(Patient patient)
    {
        try
        {
            var newPatient = new Data.Entity.Patient()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Password = patient.Password,
                Birthdate = patient.Birthdate.ToDateTime(TimeOnly.MinValue),
                Address = patient.Address,
                Gender = patient.Gender,
                Phone = patient.Phone,
                Terms = patient.Terms
            };

            await repository.AddPatientAsync(newPatient);
            return StatusCode(StatusCodes.Status201Created, MapToDTO(newPatient));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditPatient(Guid id, Patient patient)
    {
        var updateRow = await repository.UpdatePatientAsync(new()
        {
            Id = id.ToString(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            Password = patient.Password,
            Birthdate = patient.Birthdate.ToDateTime(TimeOnly.MinValue),
            Address = patient.Address,
            Gender = patient.Gender,
            Phone = patient.Phone,
            Terms = patient.Terms
        });

        if (updateRow == 0)
            return StatusCode(StatusCodes.Status404NotFound);
        var newPatient = await repository.GetPatientAsync(id);
        return StatusCode(StatusCodes.Status201Created, MapToDTO(newPatient));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        int removedPatient = await repository.DeletePatientAsync(id);
        if (removedPatient == 0)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private static PatientDTO MapToDTO(Data.Entity.Patient patient) => new()
    {
        Id = Guid.Parse(patient.Id),
        FirstName = patient.FirstName,
        LastName = patient.LastName
    };

    private static Patient MapTo(Data.Entity.Patient patient) => new()
    {
        Id = Guid.Parse(patient.Id),
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        Email = patient.Email,
        Password = patient.Password,
        Birthdate = DateOnly.FromDateTime(patient.Birthdate),
        Address = patient.Address,
        Gender = patient.Gender,
        Phone = patient.Phone,
        Terms = patient.Terms
    };
}

