using Dapper;
using PatientsApp.Data.Entity;
using System.Data;

namespace PatientsApp.Data.Repository;

public class PatientRepository : Repository
{
    public PatientRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddSamplePatientAsync()
    {
        var sql = @"INSERT INTO `Patients` (`Id`, `FirstName`, `LastName`, `Email`, `Password`, `Gender`, `Birthdate`, `Address`, `Phone`, `Terms`) VALUES
                  (@ID1, 'Mario', 'Rossi', 'mario.rossi@gmail.com', '12345678', 'Male', '1999-10-15', 'Via. A. Rossi', '+39 3547282733', true),
                  (@ID2, 'Lucia', 'Verdi', 'lucia.verdi@gmail.com', 'abcdefgh', 'Female', '1972-04-30', 'Via G. Verdi', '+39 3453829984', true),
                  (@ID3, 'Giuseppe', 'Mazzini', 'giuseppe.mazzini@gmail.com', 'grgergergrg', 'Male', '1980-08-15', 'Via M. Giove', '+39 4930284031', false);";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID1", Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@ID2", Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@ID3", Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task AddPatientAsync(Patient patient)
    {
        var sql = @"INSERT INTO `Patients` (`Id`, `FirstName`, `LastName`, `Email`, `Password`, `Gender`, `Birthdate`, `Address`, `Phone`, `Terms`) VALUES
                  (@ID, @FIRSTNAME, @LASTNAME, @EMAIL, @PASSWORD, @GENDER, @BIRTHDATE, @ADDRESS, @PHONE, @TERMS);";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", patient.Id, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@FIRSTNAME", patient.FirstName, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@LASTNAME", patient.LastName, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@EMAIL", patient.Email, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PASSWORD", patient.Password, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@GENDER", patient.Gender, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@BIRTHDATE", patient.Birthdate, DbType.Date, ParameterDirection.Input);
        dynamicParameters.Add("@ADDRESS", patient.Address, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PHONE", patient.Phone, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@TERMS", patient.Terms, DbType.Boolean, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task<IEnumerable<Patient>> GetPatientsAsync()
    {
        var sql = @"SELECT *
                    FROM Patients;";

        using var conn = GetDbConnection();
        return await conn.QueryAsync<Patient>(sql);
    }

    public async Task<Patient> GetPatientAsync(Guid id)
    {
        var sql = @"SELECT *
                    FROM Patients
                    WHERE Id = @ID;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", id.ToString(), DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<Patient>(sql, dynamicParameters);
    }

    public async Task<int> DeletePatientAsync(Guid id)
    {
        var sql = @"DELETE FROM Patients WHERE Id = @ID;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", id.ToString(), DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task<int> UpdatePatientAsync(Patient newPatient)
    {
        var sql = @"UPDATE Patients
                    SET FirstName = @FIRSTNAME, LastName = @LASTNAME, Email = @EMAIL,
                        Password = @PASSWORD, Gender = @GENDER, Birthdate = @BIRTHDATE,
                        Address = @ADDRESS, Phone = @PHONE, Terms = @TERMS
                    WHERE Id = @ID;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", newPatient.Id.ToString(), DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@FIRSTNAME", newPatient.FirstName, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@LASTNAME", newPatient.LastName, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@EMAIL", newPatient.Email, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PASSWORD", newPatient.Password, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@GENDER", newPatient.Gender, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@BIRTHDATE", newPatient.Birthdate, DbType.Date, ParameterDirection.Input);
        dynamicParameters.Add("@ADDRESS", newPatient.Address, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PHONE", newPatient.Phone, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@TERMS", newPatient.Terms, DbType.Boolean, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task<IEnumerable<Patient>> SearchPatientsByNameAsync(string name)
    {
        var sql = @"SELECT *
                    FROM Patients
                    WHERE CONCAT(FirstName, ' ', LastName) LIKE @INPUT";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@INPUT", $"%{name}%", DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<Patient>(sql, dynamicParameters);
    }
}
