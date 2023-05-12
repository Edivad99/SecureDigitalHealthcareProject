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
        var sql = @"INSERT INTO `Users` (`Id`, `Email`, `Password`, `Key2FA`, `Role`) VALUES
                  (@ID, @EMAIL, @PASSWORD, @KEY2FA, @ROLE);";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", user.Id, DbType.String, ParameterDirection.Input);
        dynParam.Add("@EMAIL", user.Email, DbType.String, ParameterDirection.Input);
        dynParam.Add("@PASSWORD", user.Password, DbType.String, ParameterDirection.Input);
        dynParam.Add("@KEY2FA", user.Key2FA, DbType.String, ParameterDirection.Input);
        dynParam.Add("@ROLE", user.Role, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynParam);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var sql = @"SELECT *
                    FROM Users
                    WHERE Email = @EMAIL;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@EMAIL", email, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, dynParam);
    }
}

