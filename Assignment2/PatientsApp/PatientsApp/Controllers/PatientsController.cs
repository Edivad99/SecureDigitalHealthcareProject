using Microsoft.AspNetCore.Mvc;
using PatientsApp.Common.DTO;
using PatientsApp.Common.Models;

namespace PatientsApp.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController : ControllerBase
{
    private static List<Patient> Patients = new()
    {
        new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Mario",
            LastName = "Rossi",
            Email = "mario.rossi@gmail.com",
            Password = "12345678",
            Birthdate = new DateOnly(1999, 10, 15),
            Address = "Via A. Rossi",
            Gender = "Male",
            Phone = "+39 3547282733",
            Terms = true
        },
        new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Lucia",
            LastName = "Verdi",
            Email = "lucia.verdi@gmail.com",
            Password = "abcdefgh",
            Birthdate = new DateOnly(1972, 04, 30),
            Address = "Via G. Verdi",
            Gender = "Female",
            Phone = "+39 3453829984",
            Terms = true
        },
        new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Giuseppe",
            LastName = "Mazzini",
            Email = "giuseppe.mazzini@gmail.com",
            Password = "grgergergrg",
            Birthdate = new DateOnly(1980, 08, 15),
            Address = "Via M. Giove",
            Gender = "Male",
            Phone = "+39 4930284031",
            Terms = false
        },
    };

    public PatientsController()
    {
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPatients()
    {
        var res = Patients.Select(Create);
        return StatusCode(StatusCodes.Status200OK, res);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPatients(Guid id)
    {
        var patient = Patients.FirstOrDefault(x => x.Id == id);
        if (patient is null)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, patient);
    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AddPatient(Patient patient)
    {
        var newPatient = new Patient()
        {
            Id = Guid.NewGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            Password = patient.Password,
            Birthdate = patient.Birthdate,
            Address = patient.Address,
            Gender = patient.Gender,
            Phone = patient.Phone,
            Terms = patient.Terms
        };
        Patients.Add(newPatient);
        return StatusCode(StatusCodes.Status201Created, Create(newPatient));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult EditPatient(Guid id, Patient patient)
    {
        int removedPatient = Patients.RemoveAll(x => x.Id == id);
        if (removedPatient == 0)
            return StatusCode(StatusCodes.Status404NotFound);
        var newPatient = new Patient()
        {
            Id = id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            Password = patient.Password,
            Birthdate = patient.Birthdate,
            Address = patient.Address,
            Gender = patient.Gender,
            Phone = patient.Phone,
            Terms = patient.Terms
        };
        Patients.Add(newPatient);
        return StatusCode(StatusCodes.Status201Created, Create(newPatient));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeletePatient(Guid id)
    {
        int removedPatient = Patients.RemoveAll(x => x.Id == id);
        if (removedPatient == 0)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private static PatientDTO Create(Patient patient) => new()
    {
        Id = patient.Id,
        FirstName = patient.FirstName,
        LastName = patient.LastName
    };
}

