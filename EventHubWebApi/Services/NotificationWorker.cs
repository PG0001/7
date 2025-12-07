using EventHubLibrary.Models;
using EventHubLibrary.Repositories.Interfaces;
using EventHubWebApi.Services;
using EventHubWebApi.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubWebApi.Services
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(IServiceProvider serviceProvider, ILogger<NotificationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var scheduledJobRepo = scope.ServiceProvider.GetRequiredService<IScheduledJobRepository>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    var pendingJobs = await scheduledJobRepo.GetPendingJobsAsync();

                    foreach (var job in pendingJobs)
                    {
                        _logger.LogInformation("Processing scheduled job. JobId={JobId} User={UserId} Event={EventId}", job.JobId, job.UserId, job.EventId);

                        await notificationService.SendNotificationAsync(
                            job.UserId,
                            job.EventId,
                            "IN_APP",
                            job.NotificationType.ToString(),
                            $"Reminder for event {job.EventId}"
                        );

                        job.IsTriggered = true;
                        await scheduledJobRepo.UpdateAsync(job);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in NotificationWorker loop");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("NotificationWorker stopped.");
        }
    }
}
