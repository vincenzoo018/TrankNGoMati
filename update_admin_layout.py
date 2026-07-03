import os
layout_path = r'Areas/Admin/Views/Shared/_AdminLayout.cshtml'
with open(layout_path, 'r', encoding='utf-8') as f:
    content = f.read()
if 'href="/Admin/Templates"' not in content:
    content = content.replace('<a href="/Admin/Dashboard"', '<a href="/Admin/Templates" class="sidebar-link">\n            <i data-lucide="file-text"></i>\n            <span>Document Templates</span>\n        </a>\n        <a href="/Admin/Dashboard"')
    with open(layout_path, 'w', encoding='utf-8') as f:
        f.write(content)
