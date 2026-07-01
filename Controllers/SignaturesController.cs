using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class SignaturesController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Digital Signatures";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Signatures";
            return View();
        }
    }
}
