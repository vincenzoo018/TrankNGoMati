using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RequireLogin(SessionHelper.ROLE_ADMIN)]
    public class DashboardController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DashboardController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Dashboard";
            ViewBag.CurrentPage = "Dashboard";

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalDepts = _context.Departments.Count();
            ViewBag.TotalDocs = _context.Documents.Count();
            ViewBag.ActiveUsers = _context.Users.Count(u => u.IsActive);

            return View();
        }
    }
}
