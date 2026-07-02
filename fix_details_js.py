import os

base_dir = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas'

tracer_loop_html = '''                    @if (Model.AuditTrailEntries != null && Model.AuditTrailEntries.Any())
                    {
                        @foreach (var entry in Model.AuditTrailEntries.OrderByDescending(a => a.Timestamp))
                        {
                            var initial = entry.User?.FullName != null && entry.User.FullName.Length >= 2 
                                ? entry.User.FullName.Substring(0, 2).ToUpper() 
                                : "SYS";
                            <div style="position: relative; padding-bottom: 24px; padding-left: 24px;">
                                <div style="position: absolute; left: -16px; top: 0; width: 24px; height: 24px; border-radius: 50%; background: var(--brand); color: white; display: flex; justify-content: center; align-items: center; font-size: 10px; font-weight: 700; border: 4px solid white; box-sizing: content-box;">@initial</div>
                                <div style="font-weight: 600; font-size: 13px; color: var(--ink);">@entry.Action</div>
                                <div style="font-size: 12px; color: var(--ink-muted); margin-top: 4px;">@entry.Details</div>
                                <div style="font-size: 11px; color: var(--ink-muted); margin-top: 4px;">@entry.Timestamp.ToString("MMM dd, yyyy \u2014 h:mm tt")</div>
                            </div>
                        }
                    }
                    else
                    {
                        <div style="font-size: 12px; color: var(--ink-muted); padding-left: 24px;">No tracking history yet.</div>
                    }'''

roles = {
    'Receiving': 'Forward',
    'DepartmentHead': 'Endorse',
    'Mayor': 'Approve'
}

for role, action_name in roles.items():
    file_path = f'{base_dir}/{role}/Views/Document/Details.cshtml'
    if not os.path.exists(file_path): continue
    
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    tracer_start = content.find('<!-- Item 1 -->')
    if tracer_start != -1:
        # Split at <!-- Item 1 -->
        parts = content.split('<!-- Item 1 -->')
        # the end of the tracer section is just before </div></div></div> (end of card)
        # we can find the first </div>\n            </div>\n        </div> in parts[1]
        end_idx = parts[1].find('</div>\n            </div>\n        </div>\n    </div>')
        if end_idx == -1:
             end_idx = parts[1].find('</div>\\n            </div>\\n        </div>')
             
        # Fallback if specific whitespace doesn't match perfectly
        if end_idx == -1:
            # find index of <!-- Signature Modal -->
            sig_idx = parts[1].find('<!-- Signature Modal')
            if sig_idx != -1:
                # count backwards to the 4th div
                temp = parts[1][:sig_idx]
                end_idx = temp.rfind('</div>', 0, temp.rfind('</div>', 0, temp.rfind('</div>', 0, temp.rfind('</div>'))))
        
        if end_idx != -1:
            new_content = parts[0] + tracer_loop_html + '\n                ' + parts[1][end_idx:]
            content = new_content
            
    # Replace the JS submitSignature function
    js_start = content.find('function submitSignature() {')
    if js_start != -1:
        js_end = content.find('}', js_start)
        # find the actual closing brace for the function
        js_end = content.find('</script>', js_end)
        if js_end != -1:
            # We want to replace just the function
            func_end = content.find('}', content.find('fetch(', js_start)) + 1
            func_end = content.find('}', func_end) + 1 # Catch the outer block
            # Actually, let's just do a string replace of the old function body since it's consistent
            old_js_signature = """        function submitSignature() {
            var signatureData = canvas.toDataURL();
            
            fetch('/Receiving/Document/Sign', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'trackingNumber=' + encodeURIComponent('@Model.TrackingNumber') + '&signatureData=' + encodeURIComponent(signatureData)
            })"""
            
            new_js = f'''        function submitSignature() {{
            var signatureData = canvas.toDataURL();
            
            fetch('/{role}/Document/{action_name}', {{
                method: 'POST',
                headers: {{ 'Content-Type': 'application/x-www-form-urlencoded' }},
                body: 'trackingNumber=' + encodeURIComponent('@Model.TrackingNumber') + '&signatureData=' + encodeURIComponent(signatureData)
            }})
            .then(res => res.json())
            .then(data => {{
                if(data.success) {{
                    showToast('Document successfully processed!', 'success');
                    setTimeout(() => window.location.href = data.redirectUrl, 1500);
                }} else {{
                    showToast(data.message || 'Error processing document', 'error');
                }}
            }})
            .catch(err => showToast('Network error occurred.', 'error'));
        }}'''
            
            # Simple replace of the whole function block
            # Let's just find "function submitSignature()" and the end of the script
            parts = content.split('function submitSignature() {')
            if len(parts) > 1:
                script_end = parts[1].find('</script>')
                # We overwrite everything from function submitSignature() to </script>
                # with our new JS and </script>
                content = parts[0] + new_js + '\n    ' + parts[1][script_end:]

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

# Admin and Cart
for role in ['Admin', 'Cart']:
    file_path = f'{base_dir}/{role}/Views/Document/Details.cshtml'
    if not os.path.exists(file_path): continue
    
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    tracer_start = content.find('<!-- Item 1 -->')
    if tracer_start != -1:
        parts = content.split('<!-- Item 1 -->')
        # For Cart and Admin, they don't have Signature Modal. So end of tracer is just end of file or card.
        # Find the last 4 </div>'s
        end_idx = parts[1].rfind('</div>', 0, parts[1].rfind('</div>', 0, parts[1].rfind('</div>', 0, parts[1].rfind('</div>'))))
        if end_idx != -1:
            new_content = parts[0] + tracer_loop_html + '\n                ' + parts[1][end_idx:]
            content = new_content
            
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

print('Details.cshtml updated.')
