using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs;
using NewEra_Cash___Carry.Models;

namespace NewEra_Cash___Carry.Controllers
{
    /// <summary>
    /// Controller for managing roles and role assignments.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Role management is restricted to Admins
    public class RoleController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public RoleController(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all roles.
        /// </summary>
        /// <returns>A list of roles.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific role by ID.
        /// </summary>
        /// <param name="id">The ID of the role.</param>
        /// <returns>The requested role.</returns>
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

        /// <summary>
        /// Creates a new role.
        /// </summary>
        /// <param name="roleDto">The role details.</param>
        /// <returns>The created role.</returns>
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

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="id">The ID of the role to update.</param>
        /// <param name="roleDto">The updated role details.</param>
        /// <returns>No content.</returns>
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

        /// <summary>
        /// Deletes a specific role by ID.
        /// </summary>
        /// <param name="id">The ID of the role to delete.</param>
        /// <returns>No content.</returns>
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

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roleId">The ID of the role to assign.</param>
        /// <returns>Success message.</returns>
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

        /// <summary>
        /// Removes a role from a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roleId">The ID of the role to remove.</param>
        /// <returns>Success message.</returns>
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
