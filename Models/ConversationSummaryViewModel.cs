namespace CalendarApp.Models
{
    public class ConversationSummaryViewModel
    {
        public User User { get; set; } = null!;
        public Message? LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }
}