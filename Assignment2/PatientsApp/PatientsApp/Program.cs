using PatientsApp.Data.Repository;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("MySQL");
string storageString = builder.Configuration.GetConnectionString("StorageConnection");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Patients API",
        Version = "v1"
    });
});
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
        builder.SetIsOriginAllowed(o => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.AddBlobServiceClient(storageString);
});

builder.Services.AddSingleton(_ => new AuthRepository(connectionString));
builder.Services.AddSingleton(_ => new PatientRepository(connectionString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

