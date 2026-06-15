using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly CalendarDbContext _context;

        public HomeController(CalendarDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int userId = 1)
        {
            var user = await _context.Users
                .Include(u => u.Events)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var events = user.Events.OrderBy(e => e.StartTime).ToList();
            ViewBag.UserId = userId;
            return View(events);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
