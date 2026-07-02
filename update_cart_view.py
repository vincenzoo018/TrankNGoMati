import os, re

base_dir = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas'

# 1. Add FlagIssue to Cart Controller
cart_ctrl_path = f'{base_dir}/Cart/Controllers/DocumentController.cs'
with open(cart_ctrl_path, 'r', encoding='utf-8') as f:
    ctrl_content = f.read()

flag_method = '''
        [HttpPost]
        public IActionResult FlagIssue(string trackingNumber)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;

            doc.IsEscalated = true;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Flagged by CART",
                Details = "CART flagged this document for potential ARTA compliance issues.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Your document ({trackingNumber}) was flagged by CART for review regarding ARTA compliance.", "Flagged");

            return Json(new { success = true, redirectUrl = "/Cart/Document" });
        }
    }
}
'''

if 'FlagIssue(' not in ctrl_content:
    ctrl_content = re.sub(r'\s*\}\s*\}\s*$', flag_method, ctrl_content)
    with open(cart_ctrl_path, 'w', encoding='utf-8') as f:
        f.write(ctrl_content)

# 2. Update Cart View
cart_view_path = f'{base_dir}/Cart/Views/Document/Details.cshtml'
with open(cart_view_path, 'r', encoding='utf-8') as f:
    view_content = f.read()

clear_btn = '''        <button class="btn" onclick="flagDocument()" style="padding: 8px 16px; border: 1px solid #F59E0B; color: #F59E0B; background: white;">
            <i data-lucide="flag"></i> Flag Issue
        </button>
        <button class="btn btn-primary" onclick="clearDocument()" style="padding: 8px 16px;">
            <i data-lucide="check-circle"></i> Clear Document
        </button>'''

view_content = re.sub(r'<button class="btn" style="padding: 8px 16px; border: 1px solid #F59E0B; color: #F59E0B; background: white;">\s*<i data-lucide="flag"></i> Flag Issue\s*</button>', clear_btn, view_content, flags=re.DOTALL)

cart_scripts = '''
@section Scripts {
    <script>
        function clearDocument() {
            if(!confirm('Are you sure you want to clear this document and forward to the Mayor?')) return;
            fetch('/Cart/Document/Clear', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'trackingNumber=' + encodeURIComponent('@Model.TrackingNumber')
            })
            .then(res => res.json())
            .then(data => {
                if(data.success) {
                    showToast('Document cleared and forwarded to Mayor!', 'success');
                    setTimeout(() => window.location.href = data.redirectUrl, 1500);
                } else {
                    showToast(data.message || 'Error processing document', 'error');
                }
            });
        }
        
        function flagDocument() {
            if(!confirm('Are you sure you want to flag this document for ARTA compliance issues?')) return;
            fetch('/Cart/Document/FlagIssue', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'trackingNumber=' + encodeURIComponent('@Model.TrackingNumber')
            })
            .then(res => res.json())
            .then(data => {
                if(data.success) {
                    showToast('Document flagged successfully.', 'warning');
                    setTimeout(() => window.location.href = data.redirectUrl, 1500);
                } else {
                    showToast(data.message || 'Error processing document', 'error');
                }
            });
        }
    </script>
}
'''

if '@section Scripts' not in view_content:
    view_content += cart_scripts
    
with open(cart_view_path, 'w', encoding='utf-8') as f:
    f.write(view_content)
    
print('Cart updated successfully')
