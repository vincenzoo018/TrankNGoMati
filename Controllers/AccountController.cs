using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            ViewData["Title"] = "Sign In";
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string role)
        {
            string targetArea = role switch
            {
                "Mayor" => "Mayor",
                "ExecutiveAdmin" => "Admin",
                "RecordsOfficer" => "Receiving",
                "DepartmentHead" => "DepartmentHead",
                "ComplianceOfficer" => "Cart",
                _ => "Admin"
            };

            return RedirectToAction("Index", "Dashboard", new { area = targetArea });
        }
    }
}
