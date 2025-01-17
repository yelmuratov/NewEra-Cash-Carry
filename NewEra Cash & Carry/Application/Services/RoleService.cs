using AutoMapper;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Application.Interfaces.RoleInterfaces;
using NewEra_Cash___Carry.Application.Interfaces.UserInterfaces;
using NewEra_Cash___Carry.Core.DTOs.role;
using NewEra_Cash___Carry.Core.Entities;

namespace NewEra_Cash___Carry.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IUserRepository userRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Role not found.");
        }

        public async Task<Role> CreateRoleAsync(RoleDto roleDto)
        {
            if (await _roleRepository.RoleExistsAsync(roleDto.Name))
            {
                throw new InvalidOperationException("A role with this name already exists.");
            }

            var role = _mapper.Map<Role>(roleDto);
            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();
            return role;
        }

        public async Task UpdateRoleAsync(int id, RoleDto roleDto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) throw new KeyNotFoundException("Role not found.");

            role.Name = roleDto.Name;
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) throw new KeyNotFoundException("Role not found.");

            _roleRepository.Delete(role);
            await _roleRepository.SaveChangesAsync();
        }

        public async Task AssignRoleToUserAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found.");

            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
            {
                throw new InvalidOperationException("User already has this role.");
            }

            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) throw new KeyNotFoundException("Role not found.");

            user.UserRoles.Add(new UserRole { RoleId = roleId });
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found.");

            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null) throw new InvalidOperationException("User does not have this role.");

            user.UserRoles.Remove(userRole);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}
