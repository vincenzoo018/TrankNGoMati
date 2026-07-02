import os

base_dir = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas'
roles = ['Admin', 'Cart', 'DepartmentHead', 'Mayor', 'Receiving']

sidebar_logout = '''        <div style="margin-top: auto; padding: 24px; padding-bottom: 32px;">
            <a href="/Account/Logout" class="btn btn-secondary" style="width: 100%; border-color: var(--border); color: #EF4444; justify-content: center;">
                <i data-lucide="log-out"></i> Sign Out
            </a>
        </div>
    </nav>'''

for role in roles:
    layout_path = f'{base_dir}/{role}/Views/Shared/_{role}Layout.cshtml'
    if not os.path.exists(layout_path):
        print(f'Missing layout for {role}')
        continue
        
    with open(layout_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    # Fix topbar logout link
    content = content.replace('<a href="/" class="topbar-btn" title="Logout">', '<a href="/Account/Logout" class="topbar-btn" title="Logout">')
    
    # Add sidebar logout button if not present
    if 'Sign Out' not in content:
        content = content.replace('</nav>', sidebar_logout)
        
    with open(layout_path, 'w', encoding='utf-8') as f:
        f.write(content)
        
print('Updated layouts with logout buttons.')
