using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Receiving.Controllers
{
    [Area("Receiving")]
    public class DashboardController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewBag.CurrentPage = "Dashboard";
            return View();
        }    }
}
