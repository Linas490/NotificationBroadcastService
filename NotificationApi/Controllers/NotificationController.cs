using Microsoft.AspNetCore.Mvc;
using NotificationApi.Models;
using NotificationApi.Services.Interfaces;

namespace NotificationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService) => _notificationService = notificationService;

        public async Task<ActionResult<List<Notification>>> GetLastNotificationsAsync([FromQuery] int limit)
        {
            //what if -5 or string?
            //what if more that existing messages?
            var lastNotifications = await _notificationService.GetLastNotificationsAsync(limit);
            return Ok(lastNotifications);
        }

        [HttpPost]
        public async Task<ActionResult> AddNotificationAsync([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Text cannot be empty.");

            if (text.Length > 500)
                return BadRequest("Message too long.");

            var notification = new Notification
            {
                Text = text,
                Timestamp = DateTime.UtcNow
            };

            await _notificationService.AddNotificationAsync(notification);
            return Ok(notification);
        }



    }
}
