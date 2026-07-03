import os
areas = ['Admin', 'Cart', 'DepartmentHead', 'Mayor', 'Receiving']
for area in areas:
    view_path = f'Areas/{area}/Views/Document/Details.cshtml'
    if not os.path.exists(view_path): continue
    
    with open(view_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    content = content.replace('/@([a-zA-Z0-9_]+)/g', '/@@([a-zA-Z0-9_]+)/g')
    content = content.replace('\">@$1</span>', '\">@@$1</span>')
    content = content.replace('Use @username to mention', 'Use @@username to mention')
    
    with open(view_path, 'w', encoding='utf-8') as f:
        f.write(content)
