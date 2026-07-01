using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.DepartmentHead.Controllers
{
    [Area("DepartmentHead")]
    public class DashboardController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewBag.CurrentPage = "Dashboard";
            return View();
        }    }
}
