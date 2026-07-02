import re
import os

filepath = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas/Receiving/Views/Document/Index.cshtml'

with open(filepath, 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Remove the standalone "OCR Scan" button
content = re.sub(r'<button class="btn btn-secondary" onclick="openModal\(\'ocrModal\'\)">.*?OCR Scan.*?</button>', '', content, flags=re.DOTALL)

# 2. Remove the entire #ocrModal block
# The #ocrModal starts at <div class="modal-overlay" id="ocrModal"> and ends before @section Scripts
ocr_start = content.find('<div class="modal-overlay" id="ocrModal">')
ocr_end = content.find('@section Scripts')
if ocr_start != -1 and ocr_end != -1:
    content = content[:ocr_start] + content[ocr_end:]
    
# 3. Add File Input + OCR Preview inside the #registerModal form body
# find <div class="modal-body" style="padding: 24px;">
file_input_html = '''                
                <!-- File Upload & Auto OCR -->
                <div class="form-group" style="margin-bottom: 24px;">
                    <label class="form-label" style="font-weight:600; font-size:12px; margin-bottom:4px;">Document File (PDF/Image)</label>
                    <input type="file" class="form-control" name="Attachment" id="auto-ocr-file" accept=".jpg,.jpeg,.png,.pdf" required style="padding: 8px;" />
                    
                    <!-- OCR Loading & Preview Area -->
                    <div id="ocr-loading" style="display:none; margin-top:12px; font-size:12px; color:var(--brand); align-items:center; gap:8px;">
                        <i data-lucide="loader" style="width:16px;height:16px;animation:spin 1s linear infinite;"></i> Running Automatic OCR Scan...
                    </div>
                    <div id="ocr-preview" style="display:none; margin-top:12px; padding:12px; background:#EFF6FF; border:1px solid #BFDBFE; border-radius:6px; font-size:12px; color:var(--ink);">
                        <strong><i data-lucide="scan-line" style="width:14px;height:14px;display:inline-block;vertical-align:middle;"></i> OCR Result:</strong>
                        <div id="ocr-text" style="margin-top:8px; max-height:80px; overflow-y:auto; color:var(--ink-muted); font-style:italic;"></div>
                    </div>
                </div>
'''
content = content.replace('<div class="modal-body" style="padding: 24px;">', '<div class="modal-body" style="padding: 24px;">' + file_input_html)

# 4. Add the Javascript for auto-ocr
# Find <script> inside @section Scripts and insert our logic
auto_ocr_js = '''
        document.getElementById('auto-ocr-file').addEventListener('change', function(e) {
            var file = e.target.files[0];
            if (!file) return;
            
            document.getElementById('ocr-loading').style.display = 'flex';
            document.getElementById('ocr-preview').style.display = 'none';
            
            var formData = new FormData();
            formData.append('file', file);
            
            fetch('/Receiving/Document/OcrExtract', {
                method: 'POST',
                body: formData
            })
            .then(res => res.json())
            .then(data => {
                document.getElementById('ocr-loading').style.display = 'none';
                if(data.success) {
                    document.getElementById('ocr-preview').style.display = 'block';
                    document.getElementById('ocr-text').innerText = data.rawText;
                    
                    // Auto-fill Title if empty
                    var titleInput = document.getElementById('reg-title');
                    if (titleInput && !titleInput.value && data.subject) {
                        titleInput.value = data.subject;
                        showToast('Title auto-filled from OCR!', 'success');
                    }
                } else {
                    showToast('OCR Failed: ' + data.message, 'error');
                }
            })
            .catch(err => {
                document.getElementById('ocr-loading').style.display = 'none';
                showToast('OCR Error: Network issue.', 'error');
            });
        });
'''
script_start = content.find('<script>')
if script_start != -1:
    content = content[:script_start+8] + auto_ocr_js + content[script_start+8:]
    
with open(filepath, 'w', encoding='utf-8') as f:
    f.write(content)

print("Receiving Index updated with Auto-OCR logic.")
