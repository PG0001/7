using EventHubLibrary.Models;
using EventHubWebApi.Dtos;
using EventHubWebApi.Models;
using EventHubWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EventHubWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notifService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notifService, ILogger<NotificationsController> logger)
        {
            _notifService = notifService;
            _logger = logger;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _notifService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationDto dto)
        {
            if (dto == null)
                return BadRequest("Payload missing.");

            try
            {
                var result = await _notifService.SendNotificationAsync(
                    dto.UserId,
                    dto.EventId,
                    dto.Type,
                    dto.Title,
                    dto.Message,
                    dto.Email,
                    dto.Phone,
                    dto.TemplateId,
                    dto.Placeholders
                );

                return Ok(result);  // 👈 now returns detailed results
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "SendNotification failed.");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
