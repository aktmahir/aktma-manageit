using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly CalendarDbContext _context;

        public EventsController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(int userId = 1)
        {
            var events = await _context.Events
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            ViewBag.UserId = userId;
            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create(int userId = 1)
        {
            ViewBag.UserId = userId;
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userId, [Bind("UserId,Title,Description,StartTime,EndTime,Location,Category,IsAllDay,ReminderTime")] Event @event)
        {
            @event.UserId = userId;

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { userId });
            }

            ViewBag.UserId = userId;
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            ViewBag.UserId = @event.UserId;
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Title,Description,StartTime,EndTime,Location,Category,IsAllDay,ReminderTime,CreatedAt")] Event @event)
        {
            if (id != @event.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index), new { userId = @event.UserId });
            }

            ViewBag.UserId = @event.UserId;
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
                return NotFound();

            ViewBag.UserId = @event.UserId;
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                int userId = @event.UserId;
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { userId });
            }

            return NotFound();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
