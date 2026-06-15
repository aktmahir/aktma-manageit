using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.Data;
using CalendarApp.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CalendarApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : AppController
    {
        private readonly CalendarDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(CalendarDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard(int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized("Only admins can access this page");

            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.Where(u => u.IsActive).CountAsync(),
                TotalEvents = await _context.Events.CountAsync(),
                TotalMessages = await _context.Messages.CountAsync(),
                AuditLogs = await _context.AuditLogs.CountAsync()
            };

            ViewBag.AdminUserId = CurrentUserId;
            ViewBag.Stats = stats;
            return View();
        }

        // GET: Admin/ManageUsers
        public async Task<IActionResult> ManageUsers(int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            var users = await _context.Users.ToListAsync();
            ViewBag.AdminUserId = CurrentUserId;
            return View(users);
        }

        // GET: Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(int id, int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Events)
                .Include(u => u.AuditLogs)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            ViewBag.AdminUserId = CurrentUserId;
            ViewBag.Roles = Enum.GetValues(typeof(UserRole));
            return View(user);
        }

        // POST: Admin/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int userId, int roleValue, int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            var oldRole = user.Role;
            user.Role = (UserRole)roleValue;
            _context.Update(user);

            // Log audit
            var auditLog = new AuditLog
            {
                UserId = CurrentUserId,
                Action = "AssignRole",
                Details = $"Changed user {user.Name} role from {oldRole} to {user.Role}",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();
            return RedirectToAction("UserDetails", new { id = userId, adminUserId = CurrentUserId });
        }

        // POST: Admin/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId, int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            _context.Update(user);

            // Log audit
            var auditLog = new AuditLog
            {
                UserId = CurrentUserId,
                Action = "ToggleUserStatus",
                Details = $"User {user.Name} status changed to {(user.IsActive ? "Active" : "Inactive")}",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();
            return RedirectToAction("UserDetails", new { id = userId, adminUserId = CurrentUserId });
        }

        // POST: Admin/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int userId, int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            var newPassword = GenerateRandomPassword();
            user.PasswordHash = HashPassword(newPassword);
            _context.Update(user);

            // Log audit
            var auditLog = new AuditLog
            {
                UserId = CurrentUserId,
                Action = "ResetPassword",
                Details = $"Reset password for user {user.Name}",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();

            ViewBag.AdminUserId = CurrentUserId;
            ViewBag.NewPassword = newPassword;
            ViewBag.Message = $"Password reset successfully. New password: {newPassword}";
            return RedirectToAction("UserDetails", new { id = userId, adminUserId = CurrentUserId });
        }

        // GET: Admin/AuditLogs
        public async Task<IActionResult> AuditLogs(int adminUserId = 0, int page = 1)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            const int pageSize = 50;
            var logs = await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditLogViewModel
                {
                    Timestamp = a.Timestamp,
                    Action = a.Action,
                    Details = a.Details,
                    IpAddress = a.IpAddress,
                    UserName = a.User != null ? a.User.Name : null,
                    UserEmail = a.User != null ? a.User.Email : null
                })
                .ToListAsync();

            var totalLogs = await _context.AuditLogs.CountAsync();

            ViewBag.AdminUserId = CurrentUserId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalLogs / pageSize);

            return View(logs);
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId, int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            if (userId == CurrentUserId)
                return BadRequest("Cannot delete yourself");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Log audit before deletion
            var auditLog = new AuditLog
            {
                UserId = CurrentUserId,
                Action = "DeleteUser",
                Details = $"Deleted user {user.Name} ({user.Email})",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction("ManageUsers", new { adminUserId = CurrentUserId });
        }

        // GET: Admin/Analytics
        public async Task<IActionResult> Analytics(int adminUserId = 0)
        {
            var admin = await _context.Users.FindAsync(CurrentUserId);
            if (admin == null)
                return Unauthorized();

            var roleDistribution = await _context.Users
                .GroupBy(u => u.Role)
                .Select(g => new RoleCountViewModel { Role = g.Key, Count = g.Count() })
                .ToListAsync();

            var recentLogins = await _context.Users
                .Where(u => u.LastLogin != null)
                .OrderByDescending(u => u.LastLogin)
                .Take(10)
                .ToListAsync();

            var viewModel = new AnalyticsViewModel
            {
                AdminUserId = CurrentUserId,
                RoleDistribution = roleDistribution,
                RecentLogins = recentLogins
            };

            return View(viewModel);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%";
            byte[] tokenData = new byte[length];
            RandomNumberGenerator.Fill(tokenData);

            var result = new StringBuilder();
            foreach (byte b in tokenData)
            {
                result.Append(validChars[b % validChars.Length]);
            }

            return result.ToString();
        }
    }
}
