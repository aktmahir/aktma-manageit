using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Controllers
{
    public class HomeController : AppController
    {
        private readonly CalendarDbContext _context;

        public HomeController(CalendarDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? userId = null)
        {
            var targetUserId = userId ?? CurrentUserId;
            if (targetUserId != CurrentUserId && !IsAdmin)
                return Forbid();

            var user = await _context.Users
                .Include(u => u.Events)
                .FirstOrDefaultAsync(u => u.Id == targetUserId);

            if (user == null)
                return NotFound();

            var events = user.Events.OrderBy(e => e.StartTime).ToList();
            ViewBag.UserId = targetUserId;
            return View(events);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
