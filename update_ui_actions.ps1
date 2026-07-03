$areas = @("Receiving", "Admin", "Mayor", "DepartmentHead", "Cart")

foreach ($area in $areas) {
    $viewPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$area\Views\Document\Details.cshtml"
    if (Test-Path $viewPath) {
        $content = Get-Content -Path $viewPath -Raw

        # 1. Add "Assignee" field to Compact Metadata Strip
        if ($content -notmatch 'Assignee:</strong>') {
            $content = $content -replace '<strong>Current Holder:</strong>.*?</div>', "$&`n    <div><strong style=`"color: var(--ink-muted);`">Assignee:</strong> @(Model.AssignedToUser?.FullName ?? `"Unassigned`")</div>"
        }

        # 2. Replace Forward button with dynamic action bar
        $actionBar = @"
    <!-- FSM Actions -->
    <div style="background:white; border:1px solid var(--border); border-radius:8px; padding:24px; margin-bottom:24px;">
        <h3 style="margin-top:0; margin-bottom:16px; font-size:16px;">Action Required</h3>
        <div style="display:flex; gap:12px; flex-wrap:wrap;">
            <button class="btn btn-primary" onclick="executeWorkflow('Endorse')"><i data-lucide="arrow-right"></i> Endorse to Dept Head</button>
            <button class="btn btn-primary" onclick="executeWorkflow('Accept')"><i data-lucide="check"></i> Accept / Forward to CART</button>
            <button class="btn btn-primary" onclick="executeWorkflow('Review')"><i data-lucide="check-circle"></i> Complete Review / Forward to Mayor</button>
            <button class="btn btn-primary" style="background:#10B981; border-color:#059669;" onclick="executeWorkflow('Approve')"><i data-lucide="check-square"></i> Final Approve</button>
            
            <button class="btn btn-secondary" onclick="executeWorkflow('Return')"><i data-lucide="corner-up-left"></i> Return</button>
            <button class="btn btn-secondary" style="color:#DC2626; border-color:#FCA5A5; background:#FEF2F2;" onclick="executeWorkflow('Reject')"><i data-lucide="x-circle"></i> Reject</button>
            <button class="btn btn-secondary" onclick="executeWorkflow('Reassign')"><i data-lucide="user-plus"></i> Reassign</button>
        </div>
        <textarea id="wfRemarks" placeholder="Optional remarks for this action..." class="form-control" style="margin-top:16px; width:100%; height:60px; padding:12px; border:1px solid var(--border); border-radius:4px; font-family:inherit;"></textarea>
    </div>
"@
        # Remove old forward button block (including signature modal if any, we'll rebuild signature later)
        if ($content -notmatch 'executeWorkflow\(') {
            $content = $content -replace '(?s)<!-- Document Preview -->(.*?)<!-- Audit Trail & Comments -->', "<!-- Document Preview -->$1$actionBar`n    <!-- Audit Trail & Comments -->"
            # And clean up any old buttons manually if needed. Actually it's better to just inject the JS.
        }

        # 3. Inject JS for executeWorkflow
        $wfScript = @"
        function executeWorkflow(action) {
            if(action === 'Reject' || action === 'Return') {
                if(!document.getElementById('wfRemarks').value) {
                    alert('Remarks are required when returning or rejecting a document.');
                    return;
                }
            }
            
            if(confirm('Are you sure you want to ' + action + ' this document?')) {
                fetch('/$area/Document/WorkflowAction', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: new URLSearchParams({
                        trackingNumber: '@Model.TrackingNumber',
                        actionType: action,
                        remarks: document.getElementById('wfRemarks').value
                    })
                })
                .then(r => r.json())
                .then(data => {
                    if (data.success) {
                        window.location.href = data.redirectUrl;
                    } else {
                        alert(data.message);
                    }
                });
            }
        }
"@
        if ($content -notmatch 'function executeWorkflow') {
            $content = $content -replace '</script>\s*$', "$wfScript`n</script>"
        }

        Set-Content -Path $viewPath -Value $content
        Write-Host "Updated UI Actions for $area"
    }
}
