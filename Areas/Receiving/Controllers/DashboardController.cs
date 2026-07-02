using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;

namespace TrackNGoMati.Areas.Receiving.Controllers
{
    [Area("Receiving")]
    [RequireLogin(SessionHelper.ROLE_RECORDS)]
    public class DashboardController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DashboardController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Receiving Dashboard";
            ViewBag.CurrentPage = "Dashboard";

            ViewBag.TotalDocs = _context.Documents.Count();
            ViewBag.PendingDocs = _context.Documents.Count(d => d.CurrentStatus == 1 || d.CurrentStatus == 2);
            ViewBag.CompletedDocs = _context.Documents.Count(d => d.CurrentStatus == 3);

            return View();
        }
    }
}
