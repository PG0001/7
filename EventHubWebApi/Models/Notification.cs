using System;

namespace EventHubWebApi.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public int? EventId { get; set; }

    public NotificationChannel Type { get; set; } = NotificationChannel.IN_APP;

    public string? Title { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    // New tracking fields
    public int? TemplateId { get; set; }
    public string? Status { get; set; }     // Pending | Sent | Failed
    public string? ChannelResponse { get; set; }


}
