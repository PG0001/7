using System;
using System.Collections.Generic;

namespace EventHubLibrary.Models;

public partial class NotificationSetting
{
    public int SettingId { get; set; }

    public bool? IsEmailEnabled { get; set; }

    public bool? IsSmsEnabled { get; set; }

    public bool? Reminder24hrEnabled { get; set; }

    public bool? Reminder1hrEnabled { get; set; }

    public int? ReminderHoursBeforeEvent { get; set; }

    public int? ReminderHoursBeforeEvent1 { get; set; }

    public int? MaxSendAttempts { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
