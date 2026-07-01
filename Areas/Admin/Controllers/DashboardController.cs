using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewBag.CurrentPage = "Dashboard";
            return View();
        }    }
}
