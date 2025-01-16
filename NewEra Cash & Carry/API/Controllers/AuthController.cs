using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Core.DTOs.user;
using NewEra_Cash___Carry.Infrastructure.Data;
using NewEra_Cash___Carry.Shared.Settings;

namespace NewEra_Cash___Carry.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDto">The user registration details.</param>
        /// <returns>A success message if registration is successful.</returns>
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

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
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

        /// <summary>
        /// Logs in a user and generates a JWT token.
        /// </summary>
        /// <param name="userDto">The user login details.</param>
        /// <returns>A JWT token if login is successful.</returns>
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

        /// <summary>
        /// Logs out the current user by blacklisting the token.
        /// </summary>
        /// <returns>A success message if logout is successful.</returns>
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

        /// <summary>
        /// Retrieves the currently authenticated user's details.
        /// </summary>
        /// <returns>The current user's details.</returns>
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

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>A JWT token.</returns>
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
