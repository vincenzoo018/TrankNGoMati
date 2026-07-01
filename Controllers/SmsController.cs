using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class SmsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "SMS Notifications";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "SMS";
            return View();
        }
    }
}
