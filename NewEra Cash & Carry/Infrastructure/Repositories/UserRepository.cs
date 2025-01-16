using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Infrastructure.Data;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly RetailOrderingSystemDbContext _context;

        public UserRepository(RetailOrderingSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserWithRolesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task AddBlacklistedTokenAsync(BlacklistedToken blacklistedToken)
        {
            await _context.BlacklistedTokens.AddAsync(blacklistedToken); // Save the blacklisted token
        }
    }
}
