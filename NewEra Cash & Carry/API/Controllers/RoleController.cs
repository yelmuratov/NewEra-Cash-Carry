using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.DTOs.role;

namespace NewEra_Cash___Carry.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            return Ok(await _roleService.GetRoleByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleDto roleDto)
        {
            var role = await _roleService.CreateRoleAsync(roleDto);
            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto roleDto)
        {
            await _roleService.UpdateRoleAsync(id, roleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser(int userId, int roleId)
        {
            await _roleService.AssignRoleToUserAsync(userId, roleId);
            return Ok(new { message = "Role assigned to user successfully." });
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            return Ok(new { message = "Role removed from user successfully." });
        }
    }
}
