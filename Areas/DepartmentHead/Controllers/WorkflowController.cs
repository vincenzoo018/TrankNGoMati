using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.DepartmentHead.Controllers
{
    [Area("DepartmentHead")]
    public class WorkflowController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Workflow";
            ViewBag.CurrentPage = "Workflow";
            return View();
        }    }
}
