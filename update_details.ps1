$areas = @("Receiving", "Admin", "Mayor", "DepartmentHead", "Cart")

foreach ($area in $areas) {
    $viewPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$area\Views\Document\Details.cshtml"
    if (Test-Path $viewPath) {
        $content = Get-Content -Path $viewPath -Raw
        
        # 1. Inject Breadcrumbs before <!-- Page Header -->
        $breadcrumbs = @"
<div style="margin-bottom: 16px; font-size: 13px; color: var(--ink-muted);">
    <a href="/$area/Dashboard" style="color: var(--brand); text-decoration: none;">Home</a> 
    <i data-lucide="chevron-right" style="width: 14px; height: 14px; display: inline-block; vertical-align: middle;"></i> 
    <a href="/$area/Document" style="color: var(--brand); text-decoration: none;">My Documents</a>
    <i data-lucide="chevron-right" style="width: 14px; height: 14px; display: inline-block; vertical-align: middle;"></i> 
    <span style="color: var(--ink);">@Model.TrackingNumber</span>
</div>
"@
        if ($content -notmatch 'My Documents</a>') {
            $content = $content -replace '<!-- Page Header -->', "$breadcrumbs`n<!-- Page Header -->"
        }

        # 2. Inject Waiting For Badge after Compact Metadata Strip
        $waitingBadge = @"
<div class="animate-fade-in" style="background: #FFFBEB; border: 1px solid #FDE68A; color: #D97706; padding: 12px 24px; border-radius: 8px; font-size: 14px; font-weight: 700; text-align: center; margin: 0 40px 24px 40px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);">
    WAITING FOR: @(Model.CurrentOfficeName ?? "N/A")
</div>
"@
        if ($content -notmatch 'WAITING FOR:') {
            $content = $content -replace '</div>\s*<!-- Main Grid -->', "</div>`n$waitingBadge`n<!-- Main Grid -->"
        }

        Set-Content -Path $viewPath -Value $content
        Write-Host "Updated Details for $area"
    }
}
