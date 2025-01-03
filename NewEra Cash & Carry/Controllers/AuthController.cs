using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs.user;
using NewEra_Cash___Carry.Helpers;
using NewEra_Cash___Carry.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NewEra_Cash___Carry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;
        private readonly AuthSettings _authSettings;

        public AuthController(RetailOrderingSystemDbContext context, IOptions<AuthSettings> authSettings)
        {
            _context = context;
            _authSettings = authSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber))
            {
                return BadRequest(new { message = "A user with this phone number already exists." });
            }

            var user = new User
            {
                PhoneNumber = userDto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                UserRoles = new List<UserRole>()
            };

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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.PhoneNumber == userDto.PhoneNumber);

            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, dbUser.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid phone number or password." });
            }

            var token = GenerateJwtToken(dbUser);
            return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

            if (jwtToken != null)
            {
                var expiration = jwtToken.ValidTo;

                var blacklistedToken = new BlacklistedToken
                {
                    Token = token,
                    Expiration = expiration
                };

                _context.BlacklistedTokens.Add(blacklistedToken);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Successfully logged out." });
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

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

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_authSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
            };

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
    }
}
