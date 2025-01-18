using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using NewEra_Cash___Carry.API.Controllers;
using NewEra_Cash___Carry.Application.Interfaces.UserInterfaces;

namespace NewEraCashAndCarry.Tests.AuthController
{
    public class LogoutTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly NewEra_Cash___Carry.API.Controllers.AuthController _authController;

        public LogoutTests()
        {
            _mockUserService = new Mock<IUserService>();
            _authController = new NewEra_Cash___Carry.API.Controllers.AuthController(_mockUserService.Object);
        }

        [Fact]
        public async Task Logout_ShouldReturnOk_WhenTokenIsBlacklisted()
        {
            var fakeToken = "fake-jwt-token";
            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
            _authController.Request.Headers["Authorization"] = $"Bearer {fakeToken}";

            _mockUserService
                .Setup(service => service.LogoutUserAsync(fakeToken))
                .Returns(Task.CompletedTask);

            var result = await _authController.Logout();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.Contains("Successfully logged out", actionResult.Value.ToString());
        }
    }
}
