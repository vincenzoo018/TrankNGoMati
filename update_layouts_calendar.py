import os

areas = ['Admin', 'Cart', 'DepartmentHead', 'Mayor', 'Receiving']
for area in areas:
    layout_path = f'Areas/{area}/Views/Shared/_{area}Layout.cshtml'
    if not os.path.exists(layout_path): continue
    with open(layout_path, 'r', encoding='utf-8') as f:
        content = f.read()
    if 'href="/Calendar"' not in content:
        # insert after dashboard or documents
        content = content.replace('<a href="/Account/Logout"', '<a href="/Calendar" class="topbar-btn" title="Calendar Deadlines"><i data-lucide="calendar"></i></a>\n        <a href="/Account/Logout"')
        with open(layout_path, 'w', encoding='utf-8') as f:
            f.write(content)
