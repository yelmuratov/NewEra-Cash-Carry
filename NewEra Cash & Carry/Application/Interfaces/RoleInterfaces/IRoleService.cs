using NewEra_Cash___Carry.Core.DTOs.role;

namespace NewEra_Cash___Carry.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> CreateRoleAsync(RoleDto roleDto);
        Task UpdateRoleAsync(int id, RoleDto roleDto);
        Task DeleteRoleAsync(int id);
        Task AssignRoleToUserAsync(int userId, int roleId);
        Task RemoveRoleFromUserAsync(int userId, int roleId);
    }
}
