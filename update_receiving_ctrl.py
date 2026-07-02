import re

file_path = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas/Receiving/Controllers/DocumentController.cs'
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

forward_method = '''
        [HttpPost]
        public IActionResult Forward(string trackingNumber, string signatureData)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            if (doc.CurrentStepIndex != 1)
                return Json(new { success = false, message = "Document is already forwarded." });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;
            
            // FSM Transition to Dept Head (Step 2)
            doc.CurrentStepIndex = 2;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Forwarded to Dept Head",
                Details = "Receiving clerk forwarded the document to the Department Head for review.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Your document ({trackingNumber}) was verified and forwarded to the Department Head.", "Forwarded");

            return Json(new { success = true, redirectUrl = "/Receiving/Document" });
        }
    }
}
'''

content = re.sub(r'\s*\}\s*\}\s*$', forward_method, content)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)

print('Receiving Controller updated.')
