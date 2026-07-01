using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.DepartmentHead.Controllers
{
    [Area("DepartmentHead")]
    public class SignatureController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Signature";
            ViewBag.CurrentPage = "Signature";
            return View();
        }    }
}
