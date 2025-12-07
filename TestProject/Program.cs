using EventHubLibrary.Models;
using EventHubLibrary.Repositories;
using EventHubWebApi.Models;
using EventHubWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace EventHubTestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration manually (console app)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup DI and host
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // DbContext
                    services.AddDbContext<DBContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                    // Repositories
                    services.AddScoped<NotificationRepository>();
                    services.AddScoped<NotificationTemplateRepository>();
                    services.AddScoped<SettingsRepository>();
                    services.AddScoped<ScheduledJobRepository>();

                    // Email & Twilio settings
                    services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
                    services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);

                    services.Configure<TwilioSettings>(configuration.GetSection("TwilioSettings"));
                    services.AddSingleton(sp => sp.GetRequiredService<IOptions<TwilioSettings>>().Value);

                    // Services
                    services.AddScoped<NotificationService>();
                })
                .Build();

            // Create scope
            using var scope = host.Services.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
            var scheduledRepo = scope.ServiceProvider.GetRequiredService<ScheduledJobRepository>();
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            // Fetch test event for Alice
            var evt = await db.Events.Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EventId == 1);

            if (evt == null)
            {
                Console.WriteLine("Test event not found.");
                return;
            }

            // Schedule reminders (adds to ScheduledJobs)
            await notificationService.ScheduleEventRemindersAsync(evt);

            // Fetch pending jobs and trigger them immediately (simulate worker)
            var pendingJobs = await scheduledRepo.GetPendingJobsAsync();

            foreach (var job in pendingJobs)
            {
                // Dummy message logic
                string message = job.NotificationType switch
                {
                    ScheduledNotificationType.REMINDER_24HR => $"Your event '{evt.EventName}' starts in 24 hours!",
                    ScheduledNotificationType.REMINDER_1HR => $"Your event '{evt.EventName}' starts in 1 hour!",
                    _ => $"You have an upcoming event '{evt.EventName}'!"
                };

                // Email log
                Console.WriteLine($"[EMAIL] To {evt.User.Email}: {message}");

                // SMS log
                Console.WriteLine($"[SMS] To {evt.User.PhoneNumber}: {message}");

                // In-app notification
                await notificationService.SendNotificationAsync(
                    evt.UserId,
                    evt.EventId,
                    "IN_APP",
                    "Event Reminder",
                    message
                );

                // Mark job triggered
                job.IsTriggered = true;
                job.TriggeredAt = DateTime.UtcNow;
                await scheduledRepo.UpdateAsync(job);
            }

            Console.WriteLine("All notifications processed successfully.");
        }
    }
}
