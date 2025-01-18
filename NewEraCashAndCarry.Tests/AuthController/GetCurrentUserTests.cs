using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewEra_Cash___Carry.Application.Interfaces.UserInterfaces;
using NewEra_Cash___Carry.Core.DTOs.user;

namespace NewEraCashAndCarry.Tests.AuthController
{
    public class GetCurrentUserTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly NewEra_Cash___Carry.API.Controllers.AuthController _authController; // Fully qualify the AuthController

        public GetCurrentUserTests()
        {
            _mockUserService = new Mock<IUserService>();
            _authController = new NewEra_Cash___Carry.API.Controllers.AuthController(_mockUserService.Object); // Fully qualify the AuthController
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUserDetails_WhenAuthenticated()
        {
            var fakeUserId = "1";
            var fakeUser = new UserDto { Id = 1, PhoneNumber = "1234567890" };

            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
            _authController.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, fakeUserId)
            }));

            _mockUserService
                .Setup(service => service.GetUserByIdAsync(1))
                .ReturnsAsync(fakeUser);

            var result = await _authController.GetCurrentUser();

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.IsType<UserDto>(actionResult.Value);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
            _authController.HttpContext.User = new ClaimsPrincipal();

            var result = await _authController.GetCurrentUser();

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, actionResult.StatusCode);
        }
    }
}
