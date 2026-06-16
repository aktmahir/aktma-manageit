using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Controllers
{
    public class MessagesController : AppController
    {
        private readonly CalendarDbContext _context;

        public MessagesController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: Messages/Index - Show conversations list
        public async Task<IActionResult> Index()
        {
            var activeUserId = CurrentUserId;

            var conversations = await _context.Messages
                .Where(m => m.SenderId == activeUserId || m.ReceiverId == activeUserId)
                .GroupBy(m => m.SenderId == activeUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault(),
                    UnreadCount = g.Count(m => m.ReceiverId == activeUserId && !m.IsRead)
                })
                .ToListAsync();

            var users = await _context.Users
                .Where(u => u.Id != activeUserId)
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

            ViewBag.CurrentUserId = activeUserId;
            return View(conversationUsers.OrderByDescending(c => c.LastMessage?.SentAt ?? DateTime.MinValue).ToList());
        }

        // GET: Messages/Chat/5
        public async Task<IActionResult> Chat(int userId)
        {
            var activeUserId = CurrentUserId;
            if (userId == activeUserId)
                return BadRequest("Cannot chat with yourself");

            var otherUser = await _context.Users.FindAsync(userId);
            if (otherUser == null)
                return NotFound();

            var messages = await _context.Messages
                .Where(m => (m.SenderId == activeUserId && m.ReceiverId == userId) ||
                            (m.SenderId == userId && m.ReceiverId == activeUserId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Mark received messages as read (acceptable UX pattern for chat apps)
            var unreadMessages = messages.Where(m => m.ReceiverId == activeUserId && !m.IsRead).ToList();
            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
            }
            if (unreadMessages.Any())
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Log error but continue to show messages
                }
            }

            ViewBag.CurrentUserId = activeUserId;
            ViewBag.OtherUserId = userId;
            ViewBag.OtherUserName = otherUser.Name;

            return View(messages);
        }

        // POST: Messages/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int senderId, int receiverId, string content)
        {
            if (senderId != CurrentUserId && !IsAdmin)
                return Forbid();

            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Message cannot be empty");

            if (senderId == receiverId)
                return BadRequest("Cannot send message to yourself");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", new { userId = receiverId });
        }

        // POST: Messages/DeleteMessage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return NotFound();

            if (message.SenderId != CurrentUserId && !IsAdmin)
                return Unauthorized();

            int otherUserId = message.SenderId == CurrentUserId ? message.ReceiverId : message.SenderId;
            
            try
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete message. Please try again.");
            }

            return RedirectToAction("Chat", new { userId = otherUserId });
        }

        // GET: Messages/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var unreadCount = await _context.Messages
                .Where(m => m.ReceiverId == CurrentUserId && !m.IsRead)
                .CountAsync();

            return Json(new { count = unreadCount });
        }
    }
}
