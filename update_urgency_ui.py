import os

view_path = r'Areas\Receiving\Views\Document\Index.cshtml'
if os.path.exists(view_path):
    with open(view_path, 'r', encoding='utf-8') as f:
        content = f.read()

    urgency_html = '''
                  <div class="form-row" style="margin-bottom: 16px;">
                      <div class="form-group" style="flex:1;">
                          <label class="form-label" style="display:flex; align-items:center; gap:8px;">
                              <input type="checkbox" name="IsUrgent" id="isUrgent" value="true" onchange="document.getElementById('urgencyJustificationWrapper').style.display = this.checked ? 'block' : 'none';" />
                              <span style="color:#EF4444; font-weight:600;"><i data-lucide="alert-triangle" style="width:14px; height:14px;"></i> Mark as Urgent / Rush (Halves ARTA SLA)</span>
                          </label>
                      </div>
                  </div>
                  <div class="form-row" id="urgencyJustificationWrapper" style="margin-bottom: 16px; display:none;">
                      <div class="form-group" style="flex:1;">
                          <label class="form-label">Urgency Justification</label>
                          <textarea class="form-control" name="UrgencyJustification" rows="2" placeholder="Required if marked urgent..."></textarea>
                      </div>
                  </div>
'''

    if 'name="IsUrgent"' not in content:
        # Find where to inject it. Before the Contact Number input or after Document Type
        if '<div class="form-row" style="margin-bottom: 16px;">' in content:
            # Let's just insert it after the auto-ocr-file block
            target = '<!-- OCR Loading & Preview Area -->'
            if target in content:
                content = content.replace(target, urgency_html + target)
                with open(view_path, 'w', encoding='utf-8') as f:
                    f.write(content)
