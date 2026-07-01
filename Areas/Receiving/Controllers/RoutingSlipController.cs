using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Receiving.Controllers
{
    [Area("Receiving")]
    public class RoutingSlipController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "RoutingSlip";
            ViewBag.CurrentPage = "RoutingSlip";
            return View();
        }    }
}
