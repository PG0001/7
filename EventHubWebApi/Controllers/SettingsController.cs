using EventHubLibrary.Repositories;
using EventHubLibrary.Repositories.Interfaces;
using EventHubWebApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventHubWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepo;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ISettingsRepository settingsRepo, ILogger<SettingsController> logger)
        {
            _settingsRepo = settingsRepo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            try
            {
                var settings = await _settingsRepo.GetAsync();
                if (settings == null) return NotFound();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notification settings");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NotificationSettingsDto dto)
        {
            try
            {
                var settings = await _settingsRepo.GetAsync();
                if (settings == null) return NotFound();

                settings.IsEmailEnabled = dto.IsEmailEnabled;
                settings.IsSmsEnabled = dto.IsSmsEnabled;
                settings.Reminder24hrEnabled = dto.Reminder24hrEnabled;
                settings.Reminder1hrEnabled = dto.Reminder1hrEnabled;
                settings.ReminderHoursBeforeEvent = dto.ReminderHoursBeforeEvent;
                settings.ReminderHoursBeforeEvent1 = dto.ReminderHoursBeforeEvent1;

                await _settingsRepo.UpdateAsync(settings);
                _logger.LogInformation("Notification settings updated");

                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification settings");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
