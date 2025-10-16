using Nexon.FleaMarket.Domain.Entities;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
}