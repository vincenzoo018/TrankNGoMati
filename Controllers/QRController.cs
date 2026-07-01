using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class QRController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "QR Code Tracking";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "QR";
            return View();
        }
    }
}
