using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Application.Interfaces.RoleInterfaces;
using NewEra_Cash___Carry.Infrastructure.Data;

namespace NewEra_Cash___Carry.Infrastructure.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly RetailOrderingSystemDbContext _context;

        public RoleRepository(RetailOrderingSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _context.Roles.AnyAsync(r => r.Name == roleName);
        }
    }
}
