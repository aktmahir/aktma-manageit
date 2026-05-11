namespace CalendarApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public EventCategory Category { get; set; }
        public bool IsAllDay { get; set; }
        public string? ReminderTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }

    public enum EventCategory
    {
        Work,
        Personal,
        Meeting,
        Birthday,
        Holiday,
        Other
    }
}
