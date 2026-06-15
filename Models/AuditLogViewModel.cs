namespace CalendarApp.Models
{
    public class AuditLogViewModel
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }
}