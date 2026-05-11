using Microsoft.EntityFrameworkCore;
using CalendarApp.Models;

namespace CalendarApp.Data
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Event relationship
            modelBuilder.Entity<Event>()
                .HasOne(e => e.User)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Message relationships
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

            // Configure User indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed initial data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "John Doe", Email = "john@example.com", CreatedAt = DateTime.Now },
                new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", CreatedAt = DateTime.Now }
            );

            var today = DateTime.Now;
            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    UserId = 1,
                    Title = "Team Meeting",
                    Description = "Weekly team sync",
                    StartTime = today.AddDays(1).AddHours(10),
                    EndTime = today.AddDays(1).AddHours(11),
                    Location = "Conference Room A",
                    Category = EventCategory.Meeting,
                    IsAllDay = false,
                    CreatedAt = DateTime.Now
                },
                new Event
                {
                    Id = 2,
                    UserId = 1,
                    Title = "Project Deadline",
                    Description = "Final project submission",
                    StartTime = today.AddDays(5),
                    EndTime = today.AddDays(5),
                    Category = EventCategory.Work,
                    IsAllDay = true,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
