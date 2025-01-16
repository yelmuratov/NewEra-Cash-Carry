using NewEra_Cash___Carry.Core.Entities;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Application.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserWithRolesAsync(int id);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task AddBlacklistedTokenAsync(BlacklistedToken blacklistedToken); // Add this method
    }
}
