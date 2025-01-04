using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs;
using NewEra_Cash___Carry.Models;

namespace NewEra_Cash___Carry.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Role management is restricted to Admins
    public class RoleController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;

        public RoleController(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

        // Get all roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        // Get a role by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { message = "Role not found." });
            }

            return Ok(role);
        }

        // Create a new role
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleDto roleDto)
        {
            if (await _context.Roles.AnyAsync(r => r.Name == roleDto.Name))
            {
                return Conflict(new { message = "A role with this name already exists." });
            }

            var role = new Role
            {
                Name = roleDto.Name
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
        }

        // Update a role
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto roleDto)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { message = "Role not found." });
            }

            role.Name = roleDto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete a role
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { message = "Role not found." });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Assign a role to a user
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
            {
                return BadRequest(new { message = "User already has this role." });
            }

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return NotFound(new { message = "Role not found." });
            }

            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role assigned to user successfully." });
        }

        // Remove a role from a user
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null)
            {
                return BadRequest(new { message = "User does not have this role." });
            }

            user.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role removed from user successfully." });
        }
    }
}
