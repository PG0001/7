using EventHubLibrary.Models;
using EventHubLibrary.Repositories;
using EventHubLibrary.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EventHubWebApi.Services
{
    public class SchedulerService
    {
        private readonly IScheduledJobRepository _scheduledJobRepo;
        private readonly ISettingsRepository _settingsRepo;
        private readonly ILogger<SchedulerService> _logger;

        public SchedulerService(
            IScheduledJobRepository scheduledJobRepo,
            ISettingsRepository settingsRepo,
            ILogger<SchedulerService> logger)
        {
            _scheduledJobRepo = scheduledJobRepo;
            _settingsRepo = settingsRepo;
            _logger = logger;
        }

        public async Task ScheduleEventRemindersAsync(Event evt)
        {
            var settings = await _settingsRepo.GetAsync();
            if (settings == null)
            {
                _logger.LogWarning("Notification settings not found. Skipping scheduling.");
                return;
            }

            var now = DateTime.UtcNow;

            // 24-hour reminder
            if (settings.Reminder24hrEnabled.GetValueOrDefault() && evt.StartTime > now.AddHours(24))
            {
                var job = new ScheduledJob
                {
                    UserId = evt.UserId,
                    EventId = evt.EventId,
                    NotificationType = ScheduledNotificationType.REMINDER_24HR,
                    ScheduledTime = evt.StartTime.AddHours(-24),
                    IsTriggered = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _scheduledJobRepo.AddScheduledJobAsync(job);
                _logger.LogInformation("Scheduled 24hr reminder. User={UserId} Event={EventId} ScheduledAt={Time}", job.UserId, job.EventId, job.ScheduledTime);
            }

            // 1-hour reminder
            if (settings.Reminder1hrEnabled.GetValueOrDefault() && evt.StartTime > now.AddHours(1))
            {
                var job = new ScheduledJob
                {
                    UserId = evt.UserId,
                    EventId = evt.EventId,
                    NotificationType = ScheduledNotificationType.REMINDER_1HR,
                    ScheduledTime = evt.StartTime.AddHours(-1),
                    IsTriggered = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _scheduledJobRepo.AddScheduledJobAsync(job);
                _logger.LogInformation("Scheduled 1hr reminder. User={UserId} Event={EventId} ScheduledAt={Time}", job.UserId, job.EventId, job.ScheduledTime);
            }
        }
    }
}
