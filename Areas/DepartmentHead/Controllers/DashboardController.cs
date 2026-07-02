using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;

namespace TrackNGoMati.Areas.DepartmentHead.Controllers
{
    [Area("DepartmentHead")]
    [RequireLogin(SessionHelper.ROLE_DEPT)]
    public class DashboardController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DashboardController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Department Head Dashboard";
            ViewBag.CurrentPage = "Dashboard";

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID);
            var user = userId != null ? _context.Users.Find(userId) : null;
            var deptId = user?.DepartmentId;

            var query = _context.Documents.AsQueryable();
            
            if (deptId != null)
            {
                query = query.Where(d => d.DepartmentId == deptId);
            }

            ViewBag.TotalDeptDocs = query.Count();
            ViewBag.PendingReview = query.Count(d => d.CurrentStatus == 1);
            ViewBag.Endorsed = query.Count(d => d.CurrentStatus > 1);

            return View();
        }
    }
}
