import os

base_dir = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas'

# Department Head
dept_head_code = '''using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;
using System;

namespace TrackNGoMati.Areas.DepartmentHead.Controllers
{
    [Area("DepartmentHead")]
    [RequireLogin(SessionHelper.ROLE_DEPTHEAD)]
    public class DocumentController : Controller
    {
        private readonly TrackNgoDbContext _context;
        private readonly SmsService _sms;

        public DocumentController(TrackNgoDbContext context, SmsService sms)
        {
            _context = context;
            _sms = sms;
        }

        public IActionResult Index(string? search = null)
        {
            ViewData["Title"] = "Department Endorsements";
            ViewBag.CurrentPage = "Document";
            
            // FSM Step 2: Department Head
            var query = _context.Documents.Where(d => d.CurrentStepIndex == 2).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.TrackingNumber.Contains(search) || d.Title.Contains(search));
            }

            return View(query.OrderByDescending(d => d.LastUpdated).ToList());
        }

        public IActionResult Details(string? id)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentPage = "Document";
            var doc = _context.Documents
                .Include(d => d.AuditTrailEntries).ThenInclude(a => a.User)
                .Include(d => d.WorkflowTransitions)
                .FirstOrDefault(d => d.TrackingNumber == id);
                
            if (doc == null) return NotFound();
            return View(doc);
        }
        
        [HttpPost]
        public IActionResult Endorse(string trackingNumber, string signatureData)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            if (doc.CurrentStepIndex != 2)
                return Json(new { success = false, message = "Document is not in the Department Head stage." });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;
            
            // Save digital signature
            _context.DigitalSignatures.Add(new DigitalSignature
            {
                DocumentId = doc.Id,
                UserId = userId,
                SignatureData = signatureData, // Assuming base64 png
                HashAlgorithm = "SHA-256",
                SignatureHash = "sha256:generated-hash-placeholder", // Real hash logic here
                SignedAt = DateTime.Now
            });

            // FSM Transition to CART (Step 3)
            doc.CurrentStepIndex = 3;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Endorsed",
                Details = "Department Head reviewed, signed, and endorsed the document to CART.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Your document ({trackingNumber}) was endorsed by the Department Head and is now with CART.", "Endorsed");

            return Json(new { success = true, redirectUrl = "/DepartmentHead/Document" });
        }
    }
}
'''

cart_code = '''using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;
using System;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    [RequireLogin(SessionHelper.ROLE_CART)]
    public class DocumentController : Controller
    {
        private readonly TrackNgoDbContext _context;
        private readonly SmsService _sms;

        public DocumentController(TrackNgoDbContext context, SmsService sms)
        {
            _context = context;
            _sms = sms;
        }

        public IActionResult Index(string? search = null)
        {
            ViewData["Title"] = "ARTA Monitoring Dashboard";
            ViewBag.CurrentPage = "Document";
            
            // CART monitors Step 3 specifically for clearing, but can see others
            var query = _context.Documents.Where(d => d.CurrentStepIndex == 3 || d.IsEscalated).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.TrackingNumber.Contains(search) || d.Title.Contains(search));
            }

            return View(query.OrderByDescending(d => d.LastUpdated).ToList());
        }

        public IActionResult Details(string? id)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentPage = "Document";
            var doc = _context.Documents
                .Include(d => d.AuditTrailEntries).ThenInclude(a => a.User)
                .Include(d => d.WorkflowTransitions)
                .FirstOrDefault(d => d.TrackingNumber == id);
                
            if (doc == null) return NotFound();
            return View(doc);
        }
        
        [HttpPost]
        public IActionResult Clear(string trackingNumber)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            if (doc.CurrentStepIndex != 3)
                return Json(new { success = false, message = "Document is not in the CART stage." });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;

            // FSM Transition to Mayor (Step 4)
            doc.CurrentStepIndex = 4;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Cleared by CART",
                Details = "CART reviewed the document for compliance and forwarded to Mayor.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Your document ({trackingNumber}) was cleared by CART and sent to the Mayor.", "CART Cleared");

            return Json(new { success = true, redirectUrl = "/Cart/Document" });
        }
    }
}
'''

mayor_code = '''using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;
using System;

namespace TrackNGoMati.Areas.Mayor.Controllers
{
    [Area("Mayor")]
    [RequireLogin(SessionHelper.ROLE_MAYOR)]
    public class DocumentController : Controller
    {
        private readonly TrackNgoDbContext _context;
        private readonly SmsService _sms;

        public DocumentController(TrackNgoDbContext context, SmsService sms)
        {
            _context = context;
            _sms = sms;
        }

        public IActionResult Index(string? search = null)
        {
            ViewData["Title"] = "Executive Approvals";
            ViewBag.CurrentPage = "Document";
            
            // FSM Step 4: Mayor
            var query = _context.Documents.Where(d => d.CurrentStepIndex == 4).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.TrackingNumber.Contains(search) || d.Title.Contains(search));
            }

            return View(query.OrderByDescending(d => d.LastUpdated).ToList());
        }

        public IActionResult Details(string? id)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentPage = "Document";
            var doc = _context.Documents
                .Include(d => d.AuditTrailEntries).ThenInclude(a => a.User)
                .Include(d => d.WorkflowTransitions)
                .FirstOrDefault(d => d.TrackingNumber == id);
                
            if (doc == null) return NotFound();
            return View(doc);
        }
        
        [HttpPost]
        public IActionResult Approve(string trackingNumber, string signatureData)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            if (doc.CurrentStepIndex != 4)
                return Json(new { success = false, message = "Document is not in the Mayor stage." });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;
            
            // Save digital signature
            _context.DigitalSignatures.Add(new DigitalSignature
            {
                DocumentId = doc.Id,
                UserId = userId,
                SignatureData = signatureData, // Assuming base64 png
                HashAlgorithm = "SHA-256",
                SignatureHash = "sha256:generated-hash-placeholder", // Real hash logic here
                SignedAt = DateTime.Now
            });

            // FSM Transition to Completed/Receiving for release (Step 5)
            doc.CurrentStepIndex = 5;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Approved",
                Details = "Mayor approved and signed the document. It is now ready for release.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Great news! Your document ({trackingNumber}) was approved by the Mayor. You may now claim it.", "Approved");

            return Json(new { success = true, redirectUrl = "/Mayor/Document" });
        }
    }
}
'''

admin_code = '''using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RequireLogin(SessionHelper.ROLE_ADMIN)]
    public class DocumentController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DocumentController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search = null)
        {
            ViewData["Title"] = "System Document Overview";
            ViewBag.CurrentPage = "Document";
            
            var query = _context.Documents.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.TrackingNumber.Contains(search) || d.Title.Contains(search));
            }

            return View(query.OrderByDescending(d => d.LastUpdated).ToList());
        }

        public IActionResult Details(string? id)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentPage = "Document";
            var doc = _context.Documents
                .Include(d => d.AuditTrailEntries).ThenInclude(a => a.User)
                .Include(d => d.WorkflowTransitions)
                .FirstOrDefault(d => d.TrackingNumber == id);
                
            if (doc == null) return NotFound();
            return View(doc);
        }
    }
}
'''

with open(os.path.join(base_dir, 'DepartmentHead/Controllers/DocumentController.cs'), 'w', encoding='utf-8') as f: f.write(dept_head_code)
with open(os.path.join(base_dir, 'Cart/Controllers/DocumentController.cs'), 'w', encoding='utf-8') as f: f.write(cart_code)
with open(os.path.join(base_dir, 'Mayor/Controllers/DocumentController.cs'), 'w', encoding='utf-8') as f: f.write(mayor_code)
with open(os.path.join(base_dir, 'Admin/Controllers/DocumentController.cs'), 'w', encoding='utf-8') as f: f.write(admin_code)

print('Controllers created.')
