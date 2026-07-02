import os
import re

base_dir = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas'

with open(f'{base_dir}/Receiving/Views/Document/Index.cshtml', 'r', encoding='utf-8') as f:
    idx_tmpl = f.read()

with open(f'{base_dir}/Receiving/Views/Document/Details.cshtml', 'r', encoding='utf-8') as f:
    det_tmpl = f.read()

roles = [
    {
        'name': 'Mayor',
        'idx_title': 'Executive Approvals',
        'idx_subtitle': 'Documents pending your signature',
        'det_sign_btn': 'Sign &amp; Approve',
        'det_sign_modal': 'Sign &amp; Approve Document',
        'is_admin': False,
        'is_cart': False
    },
    {
        'name': 'DepartmentHead',
        'idx_title': 'Department Endorsements',
        'idx_subtitle': 'Documents requiring department endorsement',
        'det_sign_btn': 'Sign &amp; Endorse',
        'det_sign_modal': 'Sign &amp; Endorse Document',
        'is_admin': False,
        'is_cart': False
    },
    {
        'name': 'Admin',
        'idx_title': 'System Document Overview',
        'idx_subtitle': 'Complete tracking of all registered documents',
        'det_sign_btn': '',
        'det_sign_modal': '',
        'is_admin': True,
        'is_cart': False
    },
    {
        'name': 'Cart',
        'idx_title': 'ARTA Monitoring Dashboard',
        'idx_subtitle': 'Monitor document processing compliance',
        'det_sign_btn': '',
        'det_sign_modal': '',
        'is_admin': False,
        'is_cart': True
    }
]

for role in roles:
    rname = role['name']
    
    # === INDEX.CSHTML ===
    idx_content = idx_tmpl.replace('/Receiving/', f'/{rname}/')
    idx_content = re.sub(r'<h1 class="page-title">.*?</h1>', f'<h1 class="page-title">{role["idx_title"]}</h1>', idx_content)
    idx_content = re.sub(r'<p class="page-subtitle">.*?</p>', f'<p class="page-subtitle">{role["idx_subtitle"]}</p>', idx_content)
    
    header_right_regex = r'<div class="page-header-right"[^>]*>.*?</div>'
    if role['is_admin']:
        new_hr = '''<div class="page-header-right" style="display: flex; gap: 8px;">
        <button class="btn btn-secondary" style="padding: 10px 20px;">
            <i data-lucide="download"></i> Export All
        </button>
    </div>'''
        idx_content = re.sub(header_right_regex, new_hr, idx_content, flags=re.DOTALL)
    elif role['is_cart']:
        new_hr = '''<div class="page-header-right" style="display: flex; gap: 8px;">
        <button class="btn btn-secondary" style="padding: 10px 20px;">
            <i data-lucide="file-text"></i> Generate ARTA Report
        </button>
    </div>'''
        idx_content = re.sub(header_right_regex, new_hr, idx_content, flags=re.DOTALL)
    else:
        idx_content = re.sub(header_right_regex, '<div class="page-header-right"></div>', idx_content, flags=re.DOTALL)
        
    os.makedirs(f'{base_dir}/{rname}/Views/Document', exist_ok=True)
    with open(f'{base_dir}/{rname}/Views/Document/Index.cshtml', 'w', encoding='utf-8') as f:
        f.write(idx_content)
        
    # === DETAILS.CSHTML ===
    det_content = det_tmpl.replace('/Receiving/', f'/{rname}/')
    
    if role['is_admin'] or role['is_cart']:
        # Remove signature button (Sign & Endorse)
        det_content = re.sub(r'<button[^>]*onclick="openModal\(\'signatureModal\'\)"[^>]*>.*?</button>', '', det_content, flags=re.DOTALL)
        
        # Remove modal and script
        parts = det_content.split('<!-- Signature Modal -->')
        det_content = parts[0]
        # Clean up trailing newlines
        det_content = det_content.rstrip() + "\n"
        
        if role['is_admin']:
            # Remove return button entirely
            det_content = re.sub(r'<button class="btn"[^>]*>.*?<i data-lucide="undo-2"></i> Return.*?</button>', '', det_content, flags=re.DOTALL)
        elif role['is_cart']:
            det_content = det_content.replace('<i data-lucide="undo-2"></i> Return', '<i data-lucide="flag"></i> Flag Issue')
            det_content = det_content.replace('border: 1px solid #EF4444; color: #EF4444;', 'border: 1px solid #F59E0B; color: #F59E0B;')
    else:
        det_content = det_content.replace('Sign &amp; Endorse', role['det_sign_btn'])
        det_content = det_content.replace('Sign &amp; Endorse Document', role['det_sign_modal'])

    with open(f'{base_dir}/{rname}/Views/Document/Details.cshtml', 'w', encoding='utf-8') as f:
        f.write(det_content)

print('Success')
