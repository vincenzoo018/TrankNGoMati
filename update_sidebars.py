import os
import re

base_dir = 'c:/Users/Huawei/source/repos/TrackNGoMati/Areas'

sidebars = {
    'Admin': '''        <div class="sidebar-nav">
            <a href="/Admin/Dashboard" class="sidebar-nav-item @(currentPage == "Dashboard" ? "active" : "")"><i data-lucide="layout-dashboard"></i> Dashboard</a>
            <a href="/Admin/Document" class="sidebar-nav-item @(currentPage == "Document" ? "active" : "")"><i data-lucide="file-text"></i> All Documents</a>
            <a href="/Admin/Users" class="sidebar-nav-item @(currentPage == "Users" ? "active" : "")"><i data-lucide="users"></i> User Management</a>
            <a href="/Admin/Archive" class="sidebar-nav-item @(currentPage == "Archive" ? "active" : "")"><i data-lucide="archive"></i> Archive</a>
            <a href="/Admin/Settings" class="sidebar-nav-item @(currentPage == "Settings" ? "active" : "")"><i data-lucide="settings"></i> System Settings</a>
        </div>''',

    'Mayor': '''        <div class="sidebar-nav">
            <a href="/Mayor/Dashboard" class="sidebar-nav-item @(currentPage == "Dashboard" ? "active" : "")"><i data-lucide="layout-dashboard"></i> Executive Dashboard</a>
            <a href="/Mayor/Document" class="sidebar-nav-item @(currentPage == "Document" ? "active" : "")"><i data-lucide="file-check"></i> Pending Approvals</a>
            <a href="/Mayor/Escalations" class="sidebar-nav-item @(currentPage == "Escalations" ? "active" : "")"><i data-lucide="alert-triangle"></i> Escalated Cases</a>
            <a href="/Mayor/Signature" class="sidebar-nav-item @(currentPage == "Signature" ? "active" : "")"><i data-lucide="pen-tool"></i> Signatures</a>
            <a href="/Mayor/Archive" class="sidebar-nav-item @(currentPage == "Archive" ? "active" : "")"><i data-lucide="archive"></i> Archive</a>
            <a href="/Mayor/Reports" class="sidebar-nav-item @(currentPage == "Reports" ? "active" : "")"><i data-lucide="bar-chart-2"></i> Reports</a>
        </div>''',

    'DepartmentHead': '''        <div class="sidebar-nav">
            <a href="/DepartmentHead/Dashboard" class="sidebar-nav-item @(currentPage == "Dashboard" ? "active" : "")"><i data-lucide="layout-dashboard"></i> Dashboard</a>
            <a href="/DepartmentHead/Document" class="sidebar-nav-item @(currentPage == "Document" ? "active" : "")"><i data-lucide="file-text"></i> Endorsements</a>
            <a href="/DepartmentHead/Returned" class="sidebar-nav-item @(currentPage == "Returned" ? "active" : "")"><i data-lucide="corner-up-left"></i> Returned Documents</a>
            <a href="/DepartmentHead/Archive" class="sidebar-nav-item @(currentPage == "Archive" ? "active" : "")"><i data-lucide="archive"></i> Archive</a>
        </div>''',

    'Cart': '''        <div class="sidebar-nav">
            <a href="/Cart/Dashboard" class="sidebar-nav-item @(currentPage == "Dashboard" ? "active" : "")"><i data-lucide="layout-dashboard"></i> Dashboard</a>
            <a href="/Cart/Document" class="sidebar-nav-item @(currentPage == "Document" ? "active" : "")"><i data-lucide="shield-check"></i> ARTA Monitoring</a>
            <a href="/Cart/Escalations" class="sidebar-nav-item @(currentPage == "Escalations" ? "active" : "")"><i data-lucide="alert-circle"></i> Escalations</a>
            <a href="/Cart/Archive" class="sidebar-nav-item @(currentPage == "Archive" ? "active" : "")"><i data-lucide="archive"></i> Archive</a>
            <a href="/Cart/Reports" class="sidebar-nav-item @(currentPage == "Reports" ? "active" : "")"><i data-lucide="file-bar-chart"></i> Reports</a>
        </div>''',

    'Receiving': '''        <div class="sidebar-nav">
            <a href="/Receiving/Dashboard" class="sidebar-nav-item @(currentPage == "Dashboard" ? "active" : "")"><i data-lucide="layout-dashboard"></i> Dashboard</a>
            <a href="/Receiving/Document" class="sidebar-nav-item @(currentPage == "Document" ? "active" : "")"><i data-lucide="file-plus"></i> Document Registration</a>
            <a href="/Receiving/RoutingSlips" class="sidebar-nav-item @(currentPage == "RoutingSlips" ? "active" : "")"><i data-lucide="map"></i> Routing Slips</a>
            <a href="/Receiving/Archive" class="sidebar-nav-item @(currentPage == "Archive" ? "active" : "")"><i data-lucide="archive"></i> Archive</a>
        </div>'''
}

for role, nav_html in sidebars.items():
    layout_path = f'{base_dir}/{role}/Views/Shared/_{role}Layout.cshtml'
    if not os.path.exists(layout_path): continue
    
    with open(layout_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    # Replace the existing <div class="sidebar-nav"> ... </div>
    # Using regex to match from <div class="sidebar-nav"> to the matching closing div before the next element
    # Since we know the structure, we can just replace everything between <div class="sidebar-nav"> and </div>\n        <div style="margin-top: auto;
    
    start_idx = content.find('<div class="sidebar-nav">')
    end_idx = content.find('<div style="margin-top: auto;', start_idx)
    
    if start_idx != -1 and end_idx != -1:
        # Include the whitespace up to end_idx
        content = content[:start_idx] + nav_html + '\n' + content[end_idx:]
        
    with open(layout_path, 'w', encoding='utf-8') as f:
        f.write(content)

print("Sidebars updated.")
