using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }
        
        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }
        
        public string Location { get; set; } = string.Empty;
        public EventCategory Category { get; set; }
        public bool IsAllDay { get; set; }
        public string? ReminderTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

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
