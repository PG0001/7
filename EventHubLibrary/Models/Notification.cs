using System;
using System.Collections.Generic;

namespace EventHubLibrary.Models;

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

    public DateTime? CreatedAt { get; set; }
    public int? TemplateId { get; set; }
    public string? Status { get; set; }     // Pending | Sent | Failed
    public string? ChannelResponse { get; set; }

    public virtual Event? Event { get; set; }

    public virtual User User { get; set; } = null!;
}
