using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    public class AuditTrailController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "AuditTrail";
            ViewBag.CurrentPage = "AuditTrail";
            return View();
        }    }
}
