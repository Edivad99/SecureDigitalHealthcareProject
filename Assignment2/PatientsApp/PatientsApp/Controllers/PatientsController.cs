using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using PatientsApp.Common.DTO;
using PatientsApp.Common.Models;
using PatientsApp.Data.Repository;

namespace PatientsApp.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController : ControllerBase
{
    private static readonly string IMAGE_NO_AVAILABLE = "https://upload.wikimedia.org/wikipedia/commons/1/14/No_Image_Available.jpg";

    private readonly PatientRepository repository;
    private readonly BlobServiceClient blobServiceClient;

    public PatientsController(PatientRepository repository, BlobServiceClient blobServiceClient)
    {
        this.repository = repository;
        this.blobServiceClient = blobServiceClient;
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
    public async Task<IActionResult> GetPatientAsync(Guid id)
    {
        var patient = await repository.GetPatientAsync(id);
        if (patient is null)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, MapTo(patient));
    }

    [HttpGet("search/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPatientsByNameAsync(string name)
    {
        var patients = await repository.SearchPatientsByNameAsync(name);
        if (!patients.Any())
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, patients.Select(MapToDTO));
    }

    [Consumes("multipart/form-data")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPatientAsync([FromForm] PatientRequest patient)
    {
        try
        {
            var username = (patient.FirstName + patient.LastName).ToLower();
            string imageName = string.Empty;
            if (patient.Image != null)
            {
                imageName = Guid.NewGuid() + Path.GetExtension(patient.Image.FileName);
            }

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
                Terms = patient.Terms,
                ProfilePicture = patient.Image != null
                    ? $"{blobServiceClient.Uri}{username}/{imageName}"
                    : IMAGE_NO_AVAILABLE
            };
            await repository.AddPatientAsync(newPatient);

            if (patient.Image != null)
            {
                var containerClient = blobServiceClient.GetBlobContainerClient(username);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                await containerClient.UploadBlobAsync(imageName, patient.Image.OpenReadStream());
            }

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
    public async Task<IActionResult> EditPatientAsync(Guid id, Patient patient)
    {
        var updateRow = await repository.UpdatePatientAsync(new()
        {
            Id = id.ToString(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email, //IGNORE
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
    public async Task<IActionResult> DeletePatientAsync(Guid id)
    {
        var patient = await repository.GetPatientAsync(id);

        int removedPatient = await repository.DeletePatientAsync(id);
        if (removedPatient == 0)
            return StatusCode(StatusCodes.Status404NotFound);

        var username = (patient.FirstName + patient.LastName).ToLower();
        var containerClient = blobServiceClient.GetBlobContainerClient(username);
        await containerClient.DeleteIfExistsAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private static PatientMinDTO MapToDTO(Data.Entity.Patient patient) => new()
    {
        Id = Guid.Parse(patient.Id),
        FirstName = patient.FirstName,
        LastName = patient.LastName
    };

    private static PatientDTO MapTo(Data.Entity.Patient patient) => new()
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
        Terms = patient.Terms,
        ProfilePicture = patient.ProfilePicture
    };
}
