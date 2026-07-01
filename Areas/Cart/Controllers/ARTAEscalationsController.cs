using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    public class ARTAEscalationsController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "ARTAEscalations";
            ViewBag.CurrentPage = "ARTAEscalations";
            return View();
        }    }
}
