using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs.user;

namespace NewEra_Cash___Carry.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;

        public UserController(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                PhoneNumber = u.PhoneNumber,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.PhoneNumber = userDto.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
