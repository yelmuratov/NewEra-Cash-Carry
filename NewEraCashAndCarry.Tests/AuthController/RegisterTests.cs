using Moq;
using NewEra_Cash___Carry.Application.Interfaces.UserInterfaces;
using NewEra_Cash___Carry.Core.DTOs.user;
using Microsoft.AspNetCore.Mvc;

namespace NewEraCashAndCarry.Tests.AuthController
{
    public class RegisterTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly NewEra_Cash___Carry.API.Controllers.AuthController _authController;

        public RegisterTests()
        {
            _mockUserService = new Mock<IUserService>();
            _authController = new NewEra_Cash___Carry.API.Controllers.AuthController(_mockUserService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsRegisteredSuccessfully()
        {
            var userDto = new UserRegisterDto { PhoneNumber = "1234567890", Password = "password123" };

            _mockUserService
                .Setup(service => service.RegisterUserAsync(userDto))
                .Returns(Task.CompletedTask);

            var result = await _authController.Register(userDto);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.Contains("User registered successfully", actionResult.Value.ToString());
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            var userDto = new UserRegisterDto { PhoneNumber = "1234567890", Password = "password123" };

            _mockUserService
                .Setup(service => service.RegisterUserAsync(userDto))
                .Throws(new Exception("Registration failed"));

            var result = await _authController.Register(userDto);

            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Contains("Registration failed", actionResult.Value.ToString());
        }
    }
}
