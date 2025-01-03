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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

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

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // DTO for user registration
    public class UserRegisterDto
    {
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }
    }

    // DTO for user login
    public class UserLoginDto
    {
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
