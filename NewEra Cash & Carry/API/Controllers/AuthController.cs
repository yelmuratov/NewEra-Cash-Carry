using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.DTOs.user;
using System.Security.Claims;

namespace NewEra_Cash___Carry.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
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
            try
            {
                await _userService.RegisterUserAsync(userDto);
                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
            try
            {
                var token = await _userService.LoginUserAsync(userDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Logs out the current user by blacklisting the token.
        /// </summary>
        /// <returns>A success message if logout is successful.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _userService.LogoutUserAsync(token);
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

            var user = await _userService.GetUserByIdAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        /// <summary>
        /// Refreshes the JWT token for the current user.
        /// </summary>
        /// <returns>A new JWT token if the refresh is successful.</returns>
        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            // Refresh logic if required (optional implementation)
            return Ok(new { message = "Token refreshed (placeholder)." });
        }
    }
}
