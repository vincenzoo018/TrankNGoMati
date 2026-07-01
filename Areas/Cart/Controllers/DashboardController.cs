using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    public class DashboardController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewBag.CurrentPage = "Dashboard";
            return View();
        }    }
}
