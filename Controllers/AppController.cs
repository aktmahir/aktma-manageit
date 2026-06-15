using System.Security.Claims;
using CalendarApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Controllers
{
    [Authorize]
    public abstract class AppController : Controller
    {
        protected int CurrentUserId
        {
            get
            {
                var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(value, out var userId) ? userId : 0;
            }
        }

        protected UserRole CurrentUserRole
        {
            get
            {
                var value = User.FindFirstValue(ClaimTypes.Role);
                return Enum.TryParse(value, out UserRole role) ? role : UserRole.User;
            }
        }

        protected bool IsAdmin => CurrentUserRole == UserRole.Admin;
    }
}