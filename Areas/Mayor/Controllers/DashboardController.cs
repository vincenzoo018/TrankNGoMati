using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;

namespace TrackNGoMati.Areas.Mayor.Controllers
{
    [Area("Mayor")]
    [RequireLogin(SessionHelper.ROLE_MAYOR)]
    public class DashboardController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DashboardController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Mayor Dashboard";
            ViewBag.CurrentPage = "Dashboard";

            ViewBag.PendingSignatures = _context.Documents.Count(d => d.CurrentStatus == 2);
            ViewBag.ApprovedDocs = _context.Documents.Count(d => d.CurrentStatus == 3);
            ViewBag.ReturnedDocs = _context.Documents.Count(d => d.CurrentStatus == 4);

            return View();
        }
    }
}
