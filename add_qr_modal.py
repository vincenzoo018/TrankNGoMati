import os

base_dir = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas'
roles = ['Receiving', 'Mayor', 'DepartmentHead', 'Admin', 'Cart']

button_html = '''                        <td>
                            <button type="button" class="btn btn-ghost" style="padding:4px; height:auto; color:var(--brand);" onclick="showQrModal('@doc.TrackingNumber', '@doc.Title')">
                                <i data-lucide="qr-code" style="width:18px; height:18px;"></i>
                            </button>
                        </td>'''

old_html_1 = '''                        <td>
                            <i data-lucide="qr-code" style="color:var(--brand); width:18px; height:18px;"></i>
                        </td>'''

old_html_2 = '''                        <td>
                            <i data-lucide="qr-code" style="color:var(--brand); width:18px; height:18px;"></i>
                        </td>'''

modal_html = '''

<!-- QR Code Modal -->
<div class="modal-overlay" id="qrModal">
    <div class="modal animate-fade-in" style="max-width: 400px; text-align: center;">
        <div class="modal-header" style="justify-content: center; border-bottom: none; padding-bottom: 0;">
            <button class="modal-close" style="position: absolute; right: 16px; top: 16px;" onclick="closeModal('qrModal')"><i data-lucide="x"></i></button>
        </div>
        <div class="modal-body" style="padding: 24px;">
            <h3 style="margin-bottom: 8px; font-weight: 600; font-size: 18px; color: var(--ink);" id="qrDocTitle">Document Title</h3>
            <p style="color: var(--ink-muted); font-size: 13px; margin-bottom: 24px;" id="qrDocRef">REF NO.</p>
            
            <div style="background: white; padding: 16px; border-radius: 12px; display: inline-block; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1); border: 1px solid var(--border); margin-bottom: 24px;">
                <img id="qrImage" src="" alt="QR Code" style="width: 200px; height: 200px; display: block;" />
            </div>
            
            <p style="font-size: 12px; color: var(--ink-muted); margin-bottom:0;">Scan this code to quickly track or process this document from any mobile device.</p>
        </div>
    </div>
</div>

<script>
    function showQrModal(trackingNumber, title) {
        document.getElementById('qrDocTitle').textContent = title;
        document.getElementById('qrDocRef').textContent = trackingNumber;
        document.getElementById('qrImage').src = 'https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=' + encodeURIComponent(trackingNumber);
        
        openModal('qrModal');
    }
</script>
'''

for role in roles:
    file_path = f'{base_dir}/{role}/Views/Document/Index.cshtml'
    if not os.path.exists(file_path):
        print(f"File not found: {file_path}")
        continue
        
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    # Replace the table cell
    if old_html_1 in content:
        content = content.replace(old_html_1, button_html)
    elif old_html_2 in content:
        content = content.replace(old_html_2, button_html)
        
    # Add modal at the end if not already added
    if 'id="qrModal"' not in content:
        content += modal_html
        
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)
        
print("Successfully added clickable QR codes to all Index.cshtml views.")
