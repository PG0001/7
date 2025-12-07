using System;
using System.Collections.Generic;

namespace EventHubLibrary.Models;

public partial class ScheduledJob
{
    public int JobId { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    public ScheduledNotificationType NotificationType { get; set; }

    public DateTime ScheduledTime { get; set; }

    public bool? IsTriggered { get; set; }

    public DateTime? TriggeredAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
