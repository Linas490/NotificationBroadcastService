using Microsoft.AspNetCore.Mvc;
using NotificationApi.Services.Interfaces;
using Shared.Model;

namespace NotificationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService) => _notificationService = notificationService;

        [HttpGet]
        public async Task<ActionResult<List<Notification>>> GetLastNotificationsAsync([FromQuery] int limit)
        {
            if (limit <= 0)
                return BadRequest("FAILED: Limit must be greater than zero.");

            var lastNotifications = await _notificationService.GetLastNotificationsAsync(limit);
            return Ok(lastNotifications);
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> AddNotificationAsync([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("FAILED: Text cannot be empty.");

            if (text.Length > 500)
                return BadRequest("FAILED: Message too long.");

            var notification = new Notification
            {
                Text = text,
                Timestamp = DateTime.UtcNow
            };

            var success = await _notificationService.AddNotificationAsync(notification);

            if (!success)
                return StatusCode(500, "FAILED: Could not save notification.");

            return Ok(notification);
        }
    }
}
