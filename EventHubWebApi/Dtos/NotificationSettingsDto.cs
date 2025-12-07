namespace EventHubWebApi.Dtos
{
    public class NotificationSettingsDto
    {
        public bool IsEmailEnabled { get; set; }
        public bool IsSmsEnabled { get; set; }
        public bool Reminder24hrEnabled { get; set; }
        public bool Reminder1hrEnabled { get; set; }
        public int ReminderHoursBeforeEvent { get; set; }
        public int ReminderHoursBeforeEvent1 { get; set; }  
    }
}
