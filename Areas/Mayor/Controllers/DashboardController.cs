using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Mayor.Controllers
{
    [Area("Mayor")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Mayor Dashboard";
            ViewBag.CurrentPage = "Dashboard";
            return View();
        }
    }
}
