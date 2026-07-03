import os, re

areas = ['Admin', 'Cart', 'DepartmentHead', 'Mayor']

for area in areas:
    layout_path = f'Areas/{area}/Views/Shared/_{area}Layout.cshtml'
    if not os.path.exists(layout_path):
        continue
    
    with open(layout_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Check if already added
    if f'href="/{area}/RoutingSlip"' in content:
        print(f'{area} already has Routing Slips')
        continue
        
    # Find the Document link
    # For Cart it might be something like: <a href="/Cart/Document" class="sidebar-nav-item @(currentPage == "Document" ? "active" : "")"><i data-lucide="file-plus"></i> My Documents</a>
    doc_link_pattern = re.compile(rf'(<a href="/{area}/Document"[^>]*>.*?</a>)', re.DOTALL)
    
    routing_link = f'\n            <a href="/{area}/RoutingSlip" class="sidebar-nav-item @(currentPage == "RoutingSlip" ? "active" : "")"><i data-lucide="map"></i> Routing Slips</a>'
    
    # Replace the Document link with Document link + Routing Slip link
    new_content = doc_link_pattern.sub(rf'\1{routing_link}', content)
    
    if new_content != content:
        with open(layout_path, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f'Updated {area}')
    else:
        print(f'Could not find Document link for {area}')
