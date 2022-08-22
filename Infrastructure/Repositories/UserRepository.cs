using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(MyLittleStoreContext context) : base(context)
    {
    }

    public async Task<User> GetByUserNameAsync(string userName)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == userName.ToLower());
    }
}

