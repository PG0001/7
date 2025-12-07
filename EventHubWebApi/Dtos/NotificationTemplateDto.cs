namespace EventHubWebApi.Dtos
{
    public class NotificationTemplateDto
    {
        public string TemplateName { get; set; } = "";
        public string? Subject { get; set; }
        public string Body { get; set; } = "";
    }
}
