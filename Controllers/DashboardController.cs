using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index(string? role)
        {
            ViewData["Title"] = "Dashboard";
            ViewBag.CurrentRole = role ?? Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Dashboard";
            return View();
        }
    }
}
