using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Receiving.Controllers
{
    [Area("Receiving")]
    public class AuditTrailController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "AuditTrail";
            ViewBag.CurrentPage = "AuditTrail";
            return View();
        }    }
}
