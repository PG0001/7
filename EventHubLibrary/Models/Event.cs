using System;
using System.Collections.Generic;

namespace EventHubLibrary.Models;

public partial class Event
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public string EventName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<ScheduledJob> ScheduledJobs { get; set; } = new List<ScheduledJob>();

    public virtual User User { get; set; } = null!;
}
