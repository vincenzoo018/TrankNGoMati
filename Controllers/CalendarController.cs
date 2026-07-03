using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using TrackNGoMati.Services;

namespace TrackNGoMati.Controllers
{
    public class CalendarController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public CalendarController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID);
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId.Value);
            if (user == null) return RedirectToAction("Login", "Account");

            // Layout to use depends on role
            if (user.Role == SessionHelper.ROLE_ADMIN) ViewBag.Layout = "~/Areas/Admin/Views/Shared/_AdminLayout.cshtml";
            else if (user.Role == SessionHelper.ROLE_MAYOR) ViewBag.Layout = "~/Areas/Mayor/Views/Shared/_MayorLayout.cshtml";
            else if (user.Role == SessionHelper.ROLE_DEPT) ViewBag.Layout = "~/Areas/DepartmentHead/Views/Shared/_DepartmentHeadLayout.cshtml";
            else if (user.Role == SessionHelper.ROLE_CART) ViewBag.Layout = "~/Areas/Cart/Views/Shared/_CartLayout.cshtml";
            else ViewBag.Layout = "~/Areas/Receiving/Views/Shared/_ReceivingLayout.cshtml";

            return View();
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            // Active documents
            var docs = _context.Documents
                .Where(d => d.CurrentStatus == 1 && !d.IsLocked)
                .ToList();

            var events = docs.Select(d => new
            {
                id = d.Id,
                title = $"{d.TrackingNumber} - {d.Title}",
                start = d.DateFiled.ToString("yyyy-MM-dd"), // Assuming start is DateFiled
                end = d.EscalationDeadline?.ToString("yyyy-MM-dd") ?? d.DateFiled.AddDays(d.ArtaprocessingDays).ToString("yyyy-MM-dd"), // Deadline
                url = $"/{d.CurrentOfficeName.Replace(" ", "")}/Document/Details/{d.TrackingNumber}",
                color = d.IsUrgent ? "#EF4444" : "#3B82F6"
            });

            return Json(events);
        }
    }
}
