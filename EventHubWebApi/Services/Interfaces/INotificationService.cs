using EventHubLibrary.Models;
using EventHubWebApi.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHubWebApi.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResultDto> SendNotificationAsync(
           int userId,
           int? eventId,
           string type,
           string? title,
           string message,
           string? email = null,
           string? phone = null,
           int? templateId = null,
           Dictionary<string, string>? placeholders = null);

        Task ScheduleEventRemindersAsync(Event evt);
        Task<List<EventHubLibrary.Models.Notification>> GetUserNotificationsAsync(int userId);
    }
}
