using Dapper;
using PatientsApp.Data.Entity;
using System.Data;

namespace PatientsApp.Data.Repository;

public class AuthRepository : Repository
{
    public AuthRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddUserAsync(User user)
    {
        var sql = @"INSERT INTO `Users` (`Id`, `Email`, `Password`) VALUES
                  (@ID, @EMAIL, @PASSWORD);";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", user.Id, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@EMAIL", user.Email, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PASSWORD", user.Password, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var sql = @"SELECT *
                    FROM Users
                    WHERE Email = @EMAIL;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@EMAIL", email, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, dynamicParameters);
    }
}

