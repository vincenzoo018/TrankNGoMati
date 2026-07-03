using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TrackNGoMati.Models;
using QRCoder;
using System.IO;

namespace TrackNGoMati.Controllers
{
    public class TrackController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public TrackController(TrackNgoDbContext context)
        {
            _context = context;
        }

        // Public Tracking Portal
        [Route("Track/{trackingNumber}")]
        public IActionResult Index(string trackingNumber)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) {
                return View("NotFound");
            }
            
            // Do not expose internal details, just status
            return View(doc);
        }

        // Endpoint to serve QR Code image dynamically
        [Route("Track/QrCode/{trackingNumber}")]
        public IActionResult QrCode(string trackingNumber)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            
            if (doc != null && !string.IsNullOrEmpty(doc.QrcodePath) && doc.QrcodePath.StartsWith("data:image/png;base64,"))
            {
                var base64Data = doc.QrcodePath.Substring("data:image/png;base64,".Length);
                var imageBytes = Convert.FromBase64String(base64Data);
                return File(imageBytes, "image/png");
            }
            
            // If missing or old, generate on the fly
            var qrUrl = $"{Request.Scheme}://{Request.Host}/Track/{trackingNumber}";
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(20);
            
            return File(qrBytes, "image/png");
        }
    }
}
