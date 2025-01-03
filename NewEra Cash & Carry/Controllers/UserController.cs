using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.Helpers;
using NewEra_Cash___Carry.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using NewEra_Cash___Carry.DTOs.user;

namespace NewEra_Cash___Carry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;
        private readonly AuthSettings _authSettings;

        public UserController(RetailOrderingSystemDbContext context, IOptions<AuthSettings> authSettings)
        {
            _context = context;
            _authSettings = authSettings.Value;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            // Check if phone number already exists
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber))
            {
                return BadRequest(new { message = "A user with this phone number already exists." });
            }

            // Create new user
            var user = new User
            {
                PhoneNumber = userDto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                UserRoles = new List<UserRole>()
            };

            // Assign default role (User)
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null)
            {
                user.UserRoles.Add(new UserRole
                {
                    RoleId = defaultRole.Id,
                    User = user
                });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        // Login user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.PhoneNumber == userDto.PhoneNumber);

            // Validate user credentials
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, dbUser.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid phone number or password." });
            }

            // Generate JWT token
            var token = GenerateJwtToken(dbUser);
            return Ok(new { Token = token });
        }

        // Generate JWT token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_authSettings.Secret);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Id.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
        };

                // Add roles as claims
            var roles = _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)
                .ToList();

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // Get Users
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


        //Assign Role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == userId);
            var role = await _context.Roles.FindAsync(roleId);

            if (user == null || role == null)
            {
                return NotFound("User or Role not found.");
            }

            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
            {
                return BadRequest("User already has this role.");
            }

            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
            await _context.SaveChangesAsync();

            return Ok("Role assigned successfully.");
        }

    }
}
