using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Controllers
{
    public class EventsController : AppController
    {
        private readonly CalendarDbContext _context;

        public EventsController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(int? userId = null)
        {
            var targetUserId = userId ?? CurrentUserId;
            if (targetUserId != CurrentUserId && !IsAdmin)
                return Forbid();

            var events = await _context.Events
                .Where(e => e.UserId == targetUserId)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            ViewBag.UserId = targetUserId;
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

            if (@event.UserId != CurrentUserId && !IsAdmin)
                return Forbid();

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create(int userId = 1)
        {
            if (userId != CurrentUserId && !IsAdmin)
                return Forbid();

            ViewBag.UserId = userId;
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userId, [Bind("UserId,Title,Description,StartTime,EndTime,Location,Category,IsAllDay,ReminderTime")] Event @event)
        {
            if (userId != CurrentUserId && !IsAdmin)
                return Forbid();

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

            if (@event.UserId != CurrentUserId && !IsAdmin)
                return Forbid();

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

            var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (existingEvent == null)
                return NotFound();

            if (existingEvent.UserId != CurrentUserId && !IsAdmin)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    @event.UserId = existingEvent.UserId;
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

            if (@event.UserId != CurrentUserId && !IsAdmin)
                return Forbid();

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
                if (@event.UserId != CurrentUserId && !IsAdmin)
                    return Forbid();

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
