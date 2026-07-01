using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    public class NotificationController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Notification";
            ViewBag.CurrentPage = "Notification";
            return View();
        }    }
}
