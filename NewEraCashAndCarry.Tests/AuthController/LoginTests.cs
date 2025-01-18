using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using NewEra_Cash___Carry.API.Controllers;
using NewEra_Cash___Carry.Application.Interfaces.UserInterfaces;
using NewEra_Cash___Carry.Core.DTOs.user;

namespace NewEraCashAndCarry.Tests.AuthController
{
    public class LoginTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly NewEra_Cash___Carry.API.Controllers.AuthController _authController;

        public LoginTests()
        {
            _mockUserService = new Mock<IUserService>();
            _authController = new NewEra_Cash___Carry.API.Controllers.AuthController(_mockUserService.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var userDto = new UserLoginDto { PhoneNumber = "1234567890", Password = "password123" };
            var fakeToken = "fake-jwt-token";

            _mockUserService
                .Setup(service => service.LoginUserAsync(userDto))
                .ReturnsAsync(fakeToken);

            var result = await _authController.Login(userDto);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.Contains(fakeToken, actionResult.Value.ToString());
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var userDto = new UserLoginDto { PhoneNumber = "1234567890", Password = "wrongpassword" };

            _mockUserService
                .Setup(service => service.LoginUserAsync(userDto))
                .Throws(new UnauthorizedAccessException("Invalid credentials"));

            var result = await _authController.Login(userDto);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, actionResult.StatusCode);
            Assert.Contains("Invalid credentials", actionResult.Value.ToString());
        }
    }
}
