using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Mayor.Controllers
{
    [Area("Mayor")]
    public class SignatureController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Signatures";
            ViewBag.CurrentPage = "Signature";
            return View();
        }
    }
}
