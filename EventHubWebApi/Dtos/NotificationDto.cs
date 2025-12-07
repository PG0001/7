namespace EventHubWebApi.Dtos
{
    public class NotificationDto
    {
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public string Type { get; set; } = "IN_APP";
        public string? Title { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? TemplateId { get; set; }
        public Dictionary<string, string>? Placeholders { get; set; }
    }
    public class NotificationResultDto
    {
        public string InApp { get; set; } = "NotStarted";
        public string Email { get; set; } = "NotStarted";
        public string Sms { get; set; } = "NotStarted";
        public bool TemplateApplied { get; set; }
        public string FinalMessage { get; set; } = "";
    }

}
