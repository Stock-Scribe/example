using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Domain.Entities;
using Nexon.FleaMarket.Infrastructure.Repository;

namespace Nexon.FleaMarket.Infrastructure.Repositories;


public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        
        var query = "SELECT UserId AS Id, UserName AS Username, BalanceSP AS SpBalance, CreatedAt, UpdatedAt FROM Users";
        
        var users = await connection.QueryAsync<User>(query);
        
        return users.ToList();
    }
}