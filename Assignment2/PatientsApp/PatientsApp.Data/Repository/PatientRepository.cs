using Dapper;
using Dapper.Transaction;
using PatientsApp.Data.Entity;
using System.Data;

namespace PatientsApp.Data.Repository;

public class PatientRepository : Repository
{
    public PatientRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddPatientAsync(Patient patient)
    {
        var sql = @"INSERT INTO `Patients` (`Id`, `FirstName`, `LastName`, `Gender`, `Birthdate`, `Address`, `Phone`, `Terms`, `ProfilePicture`) VALUES
                  (@ID, @FIRSTNAME, @LASTNAME, @GENDER, @BIRTHDATE, @ADDRESS, @PHONE, @TERMS, @PROFILEPICTURE);";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", patient.Id, DbType.String, ParameterDirection.Input);
        dynParam.Add("@FIRSTNAME", patient.FirstName, DbType.String, ParameterDirection.Input);
        dynParam.Add("@LASTNAME", patient.LastName, DbType.String, ParameterDirection.Input);
        dynParam.Add("@GENDER", patient.Gender, DbType.String, ParameterDirection.Input);
        dynParam.Add("@BIRTHDATE", patient.Birthdate, DbType.Date, ParameterDirection.Input);
        dynParam.Add("@ADDRESS", patient.Address, DbType.String, ParameterDirection.Input);
        dynParam.Add("@PHONE", patient.Phone, DbType.String, ParameterDirection.Input);
        dynParam.Add("@TERMS", patient.Terms, DbType.Boolean, ParameterDirection.Input);
        dynParam.Add("@PROFILEPICTURE", patient.ProfilePicture, DbType.String, ParameterDirection.Input);

        var sql2 = @"INSERT INTO `Users` (`Id`, `Email`, `Password`, `Role`) VALUES
                  (@ID, @EMAIL, @PASSWORD, @ROLE);";

        var dynParam2 = new DynamicParameters();
        dynParam2.Add("@ID", patient.Id, DbType.String, ParameterDirection.Input);
        dynParam2.Add("@EMAIL", patient.Email, DbType.String, ParameterDirection.Input);
        dynParam2.Add("@PASSWORD", patient.Password, DbType.String, ParameterDirection.Input);
        dynParam2.Add("@ROLE", patient.Role, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            await transaction.ExecuteAsync(sql2, dynParam2);
            await transaction.ExecuteAsync(sql, dynParam);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            try
            {
                await transaction.RollbackAsync();
            }
            catch (Exception ex2)
            {
                throw new Exception(ex2.Message, ex2.InnerException);
            }
            throw new Exception(ex.Message, ex.InnerException);
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    public async Task<IEnumerable<Patient>> GetPatientsAsync()
    {
        var sql = @"SELECT Patients.*, Email, Password
                    FROM Patients
                    INNER JOIN Users ON Users.Id = Patients.Id;";

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<Patient>(sql);
    }

    public async Task<Patient> GetPatientAsync(Guid id)
    {
        var sql = @"SELECT Patients.*, Email, Password
                    FROM Patients
                    INNER JOIN Users ON Users.Id = Patients.Id
                    WHERE Patients.Id = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", id.ToString(), DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<Patient>(sql, dynParam);
    }

    public async Task<int> DeletePatientAsync(Guid id)
    {
        var sql = @"DELETE FROM Users WHERE Id = @ID AND ROLE = 'patient';";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", id.ToString(), DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.ExecuteAsync(sql, dynParam);
    }

    public async Task<int> UpdatePatientAsync(Patient newPatient)
    {
        var sql = @"UPDATE Patients
                    SET FirstName = @FIRSTNAME, LastName = @LASTNAME,
                        Gender = @GENDER, Birthdate = @BIRTHDATE,
                        Address = @ADDRESS, Phone = @PHONE, Terms = @TERMS
                    WHERE Id = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", newPatient.Id.ToString(), DbType.String, ParameterDirection.Input);
        dynParam.Add("@FIRSTNAME", newPatient.FirstName, DbType.String, ParameterDirection.Input);
        dynParam.Add("@LASTNAME", newPatient.LastName, DbType.String, ParameterDirection.Input);
        dynParam.Add("@GENDER", newPatient.Gender, DbType.String, ParameterDirection.Input);
        dynParam.Add("@BIRTHDATE", newPatient.Birthdate, DbType.Date, ParameterDirection.Input);
        dynParam.Add("@ADDRESS", newPatient.Address, DbType.String, ParameterDirection.Input);
        dynParam.Add("@PHONE", newPatient.Phone, DbType.String, ParameterDirection.Input);
        dynParam.Add("@TERMS", newPatient.Terms, DbType.Boolean, ParameterDirection.Input);

        var sql2 = @"UPDATE Users
                    SET Password = @PASSWORD
                    WHERE Id = @ID;";

        var dynParam2 = new DynamicParameters();
        dynParam2.Add("@ID", newPatient.Id.ToString(), DbType.String, ParameterDirection.Input);
        dynParam2.Add("@PASSWORD", newPatient.Password, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        conn.Open();
        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            int x = await transaction.ExecuteAsync(sql, dynParam);
            int y = await transaction.ExecuteAsync(sql2, dynParam2);
            await transaction.CommitAsync();
            return x == y ? x : 0;
        }
        catch (Exception ex)
        {
            try
            {
                await transaction.RollbackAsync();
            }
            catch (Exception ex2)
            {
                throw new Exception(ex2.Message, ex2.InnerException);
            }
            throw new Exception(ex.Message, ex.InnerException);
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    public async Task<IEnumerable<Patient>> SearchPatientsByNameAsync(string name)
    {
        var sql = @"SELECT Patients.*, Email, Password
                    FROM Patients
                    INNER JOIN Users ON Users.Id = Patients.Id
                    WHERE CONCAT(FirstName, ' ', LastName) LIKE @INPUT";

        var dynParam = new DynamicParameters();
        dynParam.Add("@INPUT", $"%{name}%", DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<Patient>(sql, dynParam);
    }
}
