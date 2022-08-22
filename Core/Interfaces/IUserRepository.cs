using Core.Entities;
namespace Core.Interfaces;

public interface IUserRepository : IGenericRepository<User> 
{
    Task<User> GetByUserNameAsync(string userName);
}

