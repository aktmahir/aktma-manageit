using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Controllers
{
    public class MessagesController : Controller
    {
        private readonly CalendarDbContext _context;

        public MessagesController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: Messages - Show conversations list
        public async Task<IActionResult> Index(int currentUserId = 1)
        {
            var conversations = await _context.Messages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault(),
                    UnreadCount = g.Count(m => m.ReceiverId == currentUserId && !m.IsRead)
                })
                .ToListAsync();

            var users = await _context.Users
                .Where(u => u.Id != currentUserId)
                .ToListAsync();

            var conversationUsers = new List<ConversationSummaryViewModel>();
            foreach (var user in users)
            {
                var conv = conversations.FirstOrDefault(c => c.UserId == user.Id);
                conversationUsers.Add(new ConversationSummaryViewModel
                {
                    User = user,
                    LastMessage = conv?.LastMessage,
                    UnreadCount = conv?.UnreadCount ?? 0
                });
            }

            ViewBag.CurrentUserId = currentUserId;
            return View(conversationUsers.OrderByDescending(c => c.LastMessage?.SentAt ?? DateTime.MinValue).ToList());
        }

        // GET: Messages/Chat/5
        public async Task<IActionResult> Chat(int userId, int currentUserId = 1)
        {
            if (userId == currentUserId)
                return BadRequest("Cannot chat with yourself");

            var otherUser = await _context.Users.FindAsync(userId);
            if (otherUser == null)
                return NotFound();

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                            (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Mark received messages as read
            var unreadMessages = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead).ToList();
            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
            }
            await _context.SaveChangesAsync();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.OtherUserId = userId;
            ViewBag.OtherUserName = otherUser.Name;

            return View(messages);
        }

        // POST: Messages/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int senderId, int receiverId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Message cannot be empty");

            if (senderId == receiverId)
                return BadRequest("Cannot send message to yourself");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", new { userId = receiverId, currentUserId = senderId });
        }

        // POST: Messages/DeleteMessage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id, int currentUserId, int otherUserId)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null || message.SenderId != currentUserId)
                return Unauthorized();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", new { userId = otherUserId, currentUserId });
        }

        // GET: Messages/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var unreadCount = await _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .CountAsync();

            return Json(new { count = unreadCount });
        }
    }
}
