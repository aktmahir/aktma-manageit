using CalendarApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Data
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.User)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            var createdAt = DateTime.UtcNow.AddDays(-30);
            var adminEventStart = DateTime.UtcNow.Date.AddDays(1).AddHours(9);
            var ownerEventStart = DateTime.UtcNow.Date.AddDays(2).AddHours(14);
            var memberEventStart = DateTime.UtcNow.Date.AddDays(3).AddHours(11);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@example.com",
                    PasswordHash = "pEyl0p9tq0Mgq5hkefqYWy1YSxGn2pNPfoC7FEmROgc=",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = createdAt
                },
                new User
                {
                    Id = 2,
                    Name = "John Doe",
                    Email = "john@example.com",
                    PasswordHash = "jBZwjBn5ltN4FEHkeNWMxSttZSRscKEYYhSRzX/btkE=",
                    Role = UserRole.CompanyOwner,
                    IsActive = true,
                    CreatedAt = createdAt
                },
                new User
                {
                    Id = 3,
                    Name = "Jane Smith",
                    Email = "jane@example.com",
                    PasswordHash = "r36TSvW/rhqkzPb4JWyqqybLeQqwMnhEl553qXe3j6I=",
                    Role = UserRole.Member,
                    IsActive = true,
                    CreatedAt = createdAt
                }
            );

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    UserId = 1,
                    Title = "Project kickoff",
                    Description = "Review goals and demo milestones for the local portfolio build.",
                    StartTime = adminEventStart,
                    EndTime = adminEventStart.AddHours(1),
                    Location = "Conference Room A",
                    Category = EventCategory.Meeting,
                    IsAllDay = false,
                    CreatedAt = createdAt
                },
                new Event
                {
                    Id = 2,
                    UserId = 2,
                    Title = "Customer check-in",
                    Description = "Discuss roadmap progress and upcoming investor review notes.",
                    StartTime = ownerEventStart,
                    EndTime = ownerEventStart.AddHours(2),
                    Location = "Remote",
                    Category = EventCategory.Work,
                    IsAllDay = false,
                    CreatedAt = createdAt
                },
                new Event
                {
                    Id = 3,
                    UserId = 3,
                    Title = "Team retrospective",
                    Description = "Capture feedback on the latest sprint and highlight blockers.",
                    StartTime = memberEventStart,
                    EndTime = memberEventStart.AddHours(1),
                    Location = "Design Lab",
                    Category = EventCategory.Meeting,
                    IsAllDay = false,
                    CreatedAt = createdAt
                }
            );
        }
    }
}
