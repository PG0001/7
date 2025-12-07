using EventHubLibrary.Models;
using EventHubLibrary.Repositories;
using EventHubLibrary.Repositories.Interfaces;
using EventHubWebApi.Dtos;
using EventHubWebApi.Models;
using EventHubWebApi.Services;
using EventHubWebApi.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepo;
    private readonly INotificationTemplateRepository _templateRepo;
    private readonly ISettingsRepository _settingsRepo;
    private readonly IUserRepository _userRepo;
    private readonly IScheduledJobRepository _scheduledJobRepo;
    private readonly TemplateRendererService _templateRenderer;
    private readonly EmailSettings _emailSettings;
    private readonly TwilioSettings _twilioSettings;
    private readonly ILogger<NotificationService> _logger;
    public NotificationService(
        INotificationRepository notificationRepo,
        INotificationTemplateRepository templateRepo,
        ISettingsRepository settingsRepo,
        IUserRepository userRepo,
        IScheduledJobRepository scheduledJobRepo, // interface, not concrete
        TemplateRendererService templateRenderer,
        EmailSettings emailSettings,
        TwilioSettings twilioSettings,
        ILogger<NotificationService> logger)
    {
        _notificationRepo = notificationRepo;
        _templateRepo = templateRepo;
        _settingsRepo = settingsRepo;
        _userRepo = userRepo;
        _scheduledJobRepo = scheduledJobRepo;
        _templateRenderer = templateRenderer;
        _emailSettings = emailSettings;
        _twilioSettings = twilioSettings;
        _logger = logger;

        if (!string.IsNullOrEmpty(_twilioSettings.AccountSID))
        TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);
    }

    public async Task<NotificationResultDto> SendNotificationAsync(
      int userId,
      int? eventId,
      string type,
      string? title,
      string message,
      string? email ,
      string? phone,
      int? templateId = null,
      Dictionary<string, string>? placeholders = null)
    {
        var result = new NotificationResultDto();

        try
        {
            // 1️⃣ Fetch settings
            var settings = await _settingsRepo.GetAsync();
            if (settings == null)
            {
                result.InApp = "Failed: Settings not found";
                return result;
            }

            // 2️⃣ Apply template
            if (templateId.HasValue)
            {
                var tpl = await _templateRepo.GetByIdAsync(templateId.Value);

                if (tpl != null)
                {
                    title ??= tpl.Subject;
                    message = _templateRenderer.Render(tpl.Body ?? message, placeholders);

                    result.TemplateApplied = true;
                    _logger.LogInformation("[Template Applied] Final Message: {msg}", message);
                }
            }

            result.FinalMessage = message;

            // 3️⃣ Save In-App Notification
            var notif = new EventHubLibrary.Models.Notification
            {
                UserId = userId,
                EventId = eventId,
                Type = Enum.TryParse(type, true, out EventHubLibrary.Models.NotificationChannel ch)
                        ? ch
                        : EventHubLibrary.Models.NotificationChannel.IN_APP,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow,
                Status = "Sent"
            };

            await _notificationRepo.AddAsync(notif);
            result.InApp = $"Success (NotificationId={notif.NotificationId})";

            // 4️⃣ Send Email
            if (!string.IsNullOrEmpty(email))
            {
                if (settings.IsEmailEnabled == true)
                {
                    try
                    {
                        using var client = new System.Net.Mail.SmtpClient(
                            _emailSettings.SmtpServer,
                            _emailSettings.SmtpPort)
                        {
                            Credentials = new System.Net.NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                            EnableSsl = true
                        };

                        var mail = new System.Net.Mail.MailMessage
                        {
                            From = new System.Net.Mail.MailAddress(_emailSettings.Username, _emailSettings.DisplayName),
                            Subject = title,
                            Body = message,
                            IsBodyHtml = true
                        };
                        mail.To.Add(email);

                        await client.SendMailAsync(mail);
                        result.Email = "Success";
                    }
                    catch (Exception ex)
                    {
                        result.Email = $"Failed: {ex.Message}";
                        _logger.LogError(ex, "Email sending failed.");
                    }
                }
                else
                {
                    result.Email = "Skipped: Email disabled";
                }
            }
            else
            {
                result.Email = "Skipped: No email provided";
            }

            // 5️⃣ Send SMS
            if (!string.IsNullOrEmpty(phone))
            {
                if (settings.IsSmsEnabled == true)
                {
                    try
                    {
                        var sms = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
                            body: message,
                            from: new Twilio.Types.PhoneNumber(_twilioSettings.FromNumber),
                            to: new Twilio.Types.PhoneNumber(phone)
                        );

                        result.Sms = $"Success (SID={sms.Sid})";
                    }
                    catch (Exception ex)
                    {
                        result.Sms = $"Failed: {ex.Message}";
                        _logger.LogError(ex, "SMS sending failed.");
                    }
                }
                else
                {
                    result.Sms = "Skipped: SMS disabled";
                }
            }
            else
            {
                result.Sms = "Skipped: No phone provided";
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendNotificationAsync Crash");
            result.InApp = $"Critical Failure: {ex.Message}";
            return result;
        }
    }

    public async Task<List<EventHubLibrary.Models.Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _notificationRepo.GetUserNotificationsAsync(userId);
    }


    public async Task ScheduleEventRemindersAsync(Event evt)
    {
        var settings = await _settingsRepo.GetAsync();
        if (settings == null) return;

        var now = DateTime.UtcNow;

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
            _logger.LogInformation("Scheduled 24hr reminder. EventId={EventId}", evt.EventId);
        }

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
            _logger.LogInformation("Scheduled 1hr reminder. EventId={EventId}", evt.EventId);
        }
    }
}
