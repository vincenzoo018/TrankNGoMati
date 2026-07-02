using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TrackNGoMati.Models;
using TrackNGoMati.Services;

namespace TrackNGoMati.Controllers
{
    public class AccountController : Controller
    {
        private readonly TrackNgoDbContext _context;
        private readonly IHttpContextAccessor _httpCtx;

        public AccountController(TrackNgoDbContext context, IHttpContextAccessor httpCtx)
        {
            _context  = context;
            _httpCtx  = httpCtx;
        }

        // GET /Account/Login
        public IActionResult Login()
        {
            // Already logged in → redirect to their area
            var role = _httpCtx.HttpContext?.Session.GetInt32(SessionHelper.KEY_ROLE);
            if (role != null)
            {
                var area = SessionHelper.GetAreaForRole(role.Value);
                return RedirectToAction("Index", "Dashboard", new { area });
            }
            ViewData["Title"] = "Sign In";
            return View();
        }

        // POST /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var pwHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))).ToLower();
            var user   = _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == pwHash && u.IsActive);

            if (user == null)
            {
                TempData["LoginError"] = "Invalid credentials or account is inactive.";
                ViewData["Title"]      = "Sign In";
                return View();
            }

            // Write session
            _httpCtx.HttpContext!.Session.SetInt32(SessionHelper.KEY_USER_ID, user.Id);
            _httpCtx.HttpContext!.Session.SetString(SessionHelper.KEY_FULL_NAME, user.FullName);
            _httpCtx.HttpContext!.Session.SetInt32(SessionHelper.KEY_ROLE, user.Role);
            _httpCtx.HttpContext!.Session.SetString(SessionHelper.KEY_DEPT, user.Department ?? "");

            // Update last login
            user.LastLoginAt = DateTime.Now;
            _context.SaveChanges();

            // Audit trail
            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                UserId    = user.Id,
                Action    = "Login",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                Details   = $"User {user.FullName} ({user.Username}) logged in."
            });
            _context.SaveChanges();

            var area = SessionHelper.GetAreaForRole(user.Role);
            return RedirectToAction("Index", "Dashboard", new { area });
        }

        // GET /Account/Logout
        public IActionResult Logout()
        {
            var userId = _httpCtx.HttpContext?.Session.GetInt32(SessionHelper.KEY_USER_ID);
            if (userId != null)
            {
                _context.AuditTrailEntries.Add(new AuditTrailEntry
                {
                    UserId    = userId.Value,
                    Action    = "Logout",
                    Timestamp = DateTime.Now,
                    Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                    Details   = "User logged out."
                });
                _context.SaveChanges();
            }
            _httpCtx.HttpContext?.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
