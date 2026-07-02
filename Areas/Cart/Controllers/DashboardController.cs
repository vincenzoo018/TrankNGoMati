using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;
using System;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    [RequireLogin(SessionHelper.ROLE_CART)]
    public class DashboardController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DashboardController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "CART Dashboard";
            ViewBag.CurrentPage = "Dashboard";

            var now = DateTime.Now;
            
            // Need to calculate which ones are overdue
            // Since we can't easily do it in EF without complex DATEDIFF, let's pull all non-completed docs
            var docs = _context.Documents.Where(d => d.CurrentStatus != 3 && d.CurrentStatus != 4).ToList();
            
            int overdueCount = docs.Count(d => (now - d.DateFiled).TotalDays > d.ArtaprocessingDays);

            ViewBag.TotalEscalations = _context.EscalationLogs.Count();
            ViewBag.ActiveEscalations = _context.EscalationLogs.Count(e => !e.IsResolved);
            ViewBag.OverdueDocs = overdueCount;

            return View();
        }
    }
}
