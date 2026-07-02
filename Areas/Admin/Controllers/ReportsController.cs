using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RequireLogin(SessionHelper.ROLE_ADMIN)]
    public class ReportsController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public ReportsController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Reports";
            ViewBag.CurrentPage = "Reports";

            var reports = _context.ReportLogs
                .Include(r => r.GeneratedByUser)
                .OrderByDescending(r => r.GeneratedAt)
                .ToList();

            return View(reports);
        }
    }
}
