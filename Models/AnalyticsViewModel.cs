namespace CalendarApp.Models
{
    public class AnalyticsViewModel
    {
        public int AdminUserId { get; set; }
        public List<RoleCountViewModel> RoleDistribution { get; set; } = new();
        public List<User> RecentLogins { get; set; } = new();
    }

    public class RoleCountViewModel
    {
        public UserRole Role { get; set; }
        public int Count { get; set; }
    }
}