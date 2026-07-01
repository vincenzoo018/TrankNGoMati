using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    public class ReportsController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Reports";
            ViewBag.CurrentPage = "Reports";
            return View();
        }    }
}
