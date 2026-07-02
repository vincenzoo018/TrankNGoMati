$receivingView = Get-Content -Path "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\Receiving\Views\Document\Details.cshtml" -Raw

function Update-View {
    param(
        [string]$Area,
        [string]$BackLinkUrl,
        [string]$ActionPanelContent,
        [string]$ScriptReplacements
    )
    
    $newView = $receivingView
    
    # 1. Update Back Link
    $newView = $newView -replace '/Receiving/Document', $BackLinkUrl
    
    # 2. Update Actions Panel
    # Find the actions panel in the receiving view and replace it
    $actionPanelStart = '<div class="card-body" style="padding: 16px 24px; display: flex; flex-direction: column; gap: 12px;">'
    $actionPanelEndRegex = '(?s)<div class="card-body" style="padding: 16px 24px; display: flex; flex-direction: column; gap: 12px;">.*?</div>\s*</div>\s*<!-- Document Tracer / Unified Activity -->'
    
    $replacementStr = '<div class="card-body" style="padding: 16px 24px; display: flex; flex-direction: column; gap: 12px;">' + "`n" + $ActionPanelContent + "`n" + '            </div>' + "`n" + '        </div>' + "`n`n" + '        <!-- Document Tracer / Unified Activity -->'
    $newView = $newView -replace $actionPanelEndRegex, $replacementStr

    # 3. Add Custom Scripts / Endpoint overrides
    if ($ScriptReplacements) {
        $newView = $newView -replace 'fetch\(''/Receiving/Document/Forward''', "fetch('/$Area/Document/$ScriptReplacements'"
    }

    $outPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$Area\Views\Document\Details.cshtml"
    Set-Content -Path $outPath -Value $newView
    Write-Host "Updated $outPath"
}

# --- DepartmentHead ---
$dhActions = @"
                <button class="btn btn-primary" style="width: 100%; justify-content: center;" onclick="openModal('signatureModal')">
                    <i data-lucide="pen-tool"></i> Sign &amp; Endorse
                </button>
                <button class="btn" style="width: 100%; justify-content: center; border: 1px solid #EF4444; color: #EF4444; background: white;">
                    <i data-lucide="undo-2"></i> Return Document
                </button>
                <div style="display: flex; gap: 12px; margin-top: 8px;">
                    <button class="btn btn-secondary" onclick="window.print()" style="flex: 1; justify-content: center;">
                        <i data-lucide="printer"></i> Print
                    </button>
                    <button class="btn btn-secondary" style="flex: 1; justify-content: center;">
                        <i data-lucide="download"></i> Export
                    </button>
                </div>
"@
Update-View -Area "DepartmentHead" -BackLinkUrl "/DepartmentHead/Document" -ActionPanelContent $dhActions -ScriptReplacements "Endorse'"

# --- Mayor ---
$mayorActions = @"
                <button class="btn btn-primary" style="width: 100%; justify-content: center;" onclick="openModal('signatureModal')">
                    <i data-lucide="pen-tool"></i> Approve Document
                </button>
                <div style="display: flex; gap: 12px; margin-top: 8px;">
                    <button class="btn btn-secondary" onclick="window.print()" style="flex: 1; justify-content: center;">
                        <i data-lucide="printer"></i> Print
                    </button>
                    <button class="btn btn-secondary" style="flex: 1; justify-content: center;">
                        <i data-lucide="download"></i> Export
                    </button>
                </div>
"@
Update-View -Area "Mayor" -BackLinkUrl "/Mayor/Document" -ActionPanelContent $mayorActions -ScriptReplacements "Approve'"

# --- Cart ---
$cartActions = @"
                <button class="btn btn-primary" style="width: 100%; justify-content: center;" onclick="clearDocument()">
                    <i data-lucide="check-circle-2"></i> Clear ARTA
                </button>
                <button class="btn" style="width: 100%; justify-content: center; border: 1px solid #EF4444; color: #EF4444; background: white;" onclick="flagDocument()">
                    <i data-lucide="flag"></i> Flag Issue
                </button>
                <div style="display: flex; gap: 12px; margin-top: 8px;">
                    <button class="btn btn-secondary" onclick="window.print()" style="flex: 1; justify-content: center;">
                        <i data-lucide="printer"></i> Print
                    </button>
                    <button class="btn btn-secondary" style="flex: 1; justify-content: center;">
                        <i data-lucide="download"></i> Export
                    </button>
                </div>
"@
# Cart has custom JS for clear/flag instead of signature, so we need to inject that JS manually
# I'll just use a generic update for Cart, then I'll use multi_replace for the JS part
Update-View -Area "Cart" -BackLinkUrl "/Cart/Document" -ActionPanelContent $cartActions -ScriptReplacements "Clear'"

# --- Admin ---
$adminActions = @"
                <div style="display: flex; gap: 12px; margin-top: 8px;">
                    <button class="btn btn-secondary" onclick="window.print()" style="flex: 1; justify-content: center;">
                        <i data-lucide="printer"></i> Print
                    </button>
                    <button class="btn btn-secondary" style="flex: 1; justify-content: center;">
                        <i data-lucide="download"></i> Export
                    </button>
                </div>
"@
Update-View -Area "Admin" -BackLinkUrl "/Admin/Document" -ActionPanelContent $adminActions -ScriptReplacements "Forward'"

