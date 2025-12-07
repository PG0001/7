namespace EventHubWebApi.Dtos
{
    public class EventDto
    {
        public int UserId { get; set; }
        public string EventName { get; set; } = "";
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    public class EventResponseDto
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string EventName { get; set; } = "";
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
