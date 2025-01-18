using Microsoft.AspNetCore.Mvc;
using NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces;

namespace NewEra_Cash___Carry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("email")]
        public async Task<IActionResult> NotifyByEmail(string message)
        {
            await _notificationService.NotifyByEmailAsync(message);
            return Ok(new { Message = "Notification sent via Email." });
        }

        [HttpPost("sms")]
        public async Task<IActionResult> NotifyBySms(string message)
        {
            await _notificationService.NotifyBySmsAsync(message);
            return Ok(new { Message = "Notification sent via SMS." });
        }
    }
}
