using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class DepartmentsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Department Management";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Departments";
            return View();
        }
    }
}
