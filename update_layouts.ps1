$areas = @("Admin", "Cart", "DepartmentHead", "Mayor", "Receiving")

foreach ($area in $areas) {
    $layoutPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$area\Views\Shared\_$($area)Layout.cshtml"
    if (Test-Path $layoutPath) {
        $content = Get-Content -Path $layoutPath -Raw

        # 1. Update Topbar
        $oldTopbar = '    <header class="topbar">
        <button class="topbar-btn" onclick="toggleSidebar()" title="Menu" style="margin-right:auto;"><i data-lucide="menu"></i></button>
        <button class="topbar-btn" title="Notifications"><i data-lucide="bell"></i><span class="topbar-badge"></span></button>
        <span class="topbar-separator"></span>
        <a href="/Account/Logout" class="topbar-btn" title="Logout"><i data-lucide="log-out"></i></a>
    </header>'

        $newTopbar = @"
    <header class="topbar" style="position:relative;">
        <button class="topbar-btn" onclick="toggleSidebar()" title="Menu"><i data-lucide="menu"></i></button>
        
        <form action="/$area/Document" method="get" class="topbar-search" style="margin-right:auto; margin-left:24px; display:flex; align-items:center;">
            <div class="search-input" style="width: 400px; position:relative;">
                <i data-lucide="search" style="position:absolute; left:12px; top:50%; transform:translateY(-50%); width:16px; height:16px; color:var(--ink-muted);"></i>
                <input type="text" name="search" placeholder="Search by tracking number or keyword..." style="padding-left:36px; height:36px; border-radius:18px; border:1px solid var(--border); width:100%; font-size:13px; background:#f8fafc;" />
            </div>
        </form>

        <div style="position:relative;">
            <button class="topbar-btn" title="Notifications" onclick="toggleNotifications()" id="notifBtn">
                <i data-lucide="bell"></i><span class="topbar-badge" id="notifBadge" style="display:none;"></span>
            </button>
            <div id="notifDropdown" style="display:none; position:absolute; right:0; top:48px; width:320px; background:white; border:1px solid var(--border); border-radius:8px; box-shadow:0 10px 25px -5px rgba(0,0,0,0.1); z-index:1000; overflow:hidden;">
                <div style="padding:12px 16px; border-bottom:1px solid var(--border); display:flex; justify-content:space-between; align-items:center; background:#f8fafc;">
                    <strong style="font-size:13px; color:var(--ink);">Notifications</strong>
                    <a href="javascript:void(0)" onclick="markAllRead()" style="font-size:11px; color:var(--brand); text-decoration:none;">Mark all read</a>
                </div>
                <div id="notifList" style="max-height:300px; overflow-y:auto; padding:0;">
                    <div style="padding:24px; text-align:center; color:var(--ink-muted); font-size:12px;">No new notifications</div>
                </div>
            </div>
        </div>
        
        <span class="topbar-separator"></span>
        <a href="/Account/Logout" class="topbar-btn" title="Logout"><i data-lucide="log-out"></i></a>
    </header>
"@
        
        if ($content -match '<header class="topbar">') {
            # Need to match the block properly
            $content = $content -replace '(?s)\s*<header class="topbar">.*?</header>', "`n$newTopbar"
        }

        # 2. Add Notification Javascript before </body>
        $notifScript = @"
    <script>
        function toggleNotifications() {
            var dd = document.getElementById('notifDropdown');
            dd.style.display = dd.style.display === 'none' ? 'block' : 'none';
        }
        document.addEventListener('click', function(e) {
            var dd = document.getElementById('notifDropdown');
            var btn = document.getElementById('notifBtn');
            if(dd && btn && !dd.contains(e.target) && !btn.contains(e.target)) {
                dd.style.display = 'none';
            }
        });
        function loadNotifications() {
            // Fake load for now since we haven't built the endpoint
            var badge = document.getElementById('notifBadge');
            badge.style.display = 'block';
            badge.innerText = '2';
            
            document.getElementById('notifList').innerHTML = `
                <a href="#" style="display:block; padding:12px 16px; border-bottom:1px solid var(--border); text-decoration:none; transition:background 0.2s;">
                    <div style="font-size:12px; font-weight:600; color:var(--ink); margin-bottom:4px;">Document Flagged</div>
                    <div style="font-size:11px; color:var(--ink-muted);">TNG-2026-0004 has been flagged by CART.</div>
                    <div style="font-size:10px; color:var(--brand); margin-top:6px;">10 mins ago</div>
                </a>
                <a href="#" style="display:block; padding:12px 16px; border-bottom:1px solid var(--border); text-decoration:none; transition:background 0.2s;">
                    <div style="font-size:12px; font-weight:600; color:var(--ink); margin-bottom:4px;">SLA Warning</div>
                    <div style="font-size:11px; color:var(--ink-muted);">TNG-2026-0001 is nearing its ARTA deadline.</div>
                    <div style="font-size:10px; color:var(--brand); margin-top:6px;">1 hr ago</div>
                </a>
            `;
        }
        function markAllRead() {
            document.getElementById('notifBadge').style.display = 'none';
            document.getElementById('notifList').innerHTML = '<div style="padding:24px; text-align:center; color:var(--ink-muted); font-size:12px;">No new notifications</div>';
            showToast('All notifications marked as read', 'success');
        }
        
        // Load on start
        setTimeout(loadNotifications, 1000);
    </script>
</body>
"@
        
        $content = $content -replace '</body>', $notifScript

        Set-Content -Path $layoutPath -Value $content
        Write-Host "Updated layout for $area"
    }
}
