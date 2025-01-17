namespace NewEra_Cash___Carry.Application.Interfaces.RoleInterfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<bool> RoleExistsAsync(string roleName);
    }
}
