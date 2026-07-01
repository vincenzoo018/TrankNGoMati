using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Reports";
            ViewBag.CurrentPage = "Reports";
            return View();
        }    }
}
