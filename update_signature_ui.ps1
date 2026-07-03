$areas = @("Admin", "Mayor")

foreach ($area in $areas) {
    $viewPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$area\Views\Document\Details.cshtml"
    if (Test-Path $viewPath) {
        $content = Get-Content -Path $viewPath -Raw

        # 1. Inject Signature Modal UI
        $signatureModal = @"
<!-- Digital Signature Modal -->
<div class="modal-overlay" id="signatureModal" style="display:none; position:fixed; top:0; left:0; right:0; bottom:0; background:rgba(0,0,0,0.5); z-index:9999; align-items:center; justify-content:center;">
    <div class="modal-content" style="background:white; padding:24px; border-radius:8px; width:400px; box-shadow:0 10px 25px rgba(0,0,0,0.2);">
        <h3 style="margin-top:0;">E-Signature Approval</h3>
        <p style="font-size:12px; color:var(--ink-muted); margin-bottom:16px;">Please provide your digital signature below to authorize this document's final approval.</p>
        <canvas id="signatureCanvas" style="border: 1px dashed var(--border); border-radius:4px; width:100%; height:200px; background:#f8fafc; cursor:crosshair;"></canvas>
        <div style="display:flex; justify-content:space-between; margin-top:16px;">
            <button class="btn btn-secondary" type="button" onclick="clearSignature()">Clear</button>
            <div>
                <button class="btn btn-secondary" type="button" onclick="document.getElementById('signatureModal').style.display='none'">Cancel</button>
                <button class="btn btn-primary" type="button" onclick="submitSignature()">Sign & Approve</button>
            </div>
        </div>
    </div>
</div>
<script src="https://cdn.jsdelivr.net/npm/signature_pad@4.1.7/dist/signature_pad.umd.min.js"></script>
<script>
    var signaturePad;
    setTimeout(() => {
        var canvas = document.getElementById('signatureCanvas');
        if (canvas) {
            // Adjust canvas size for correct drawing scale
            canvas.width = canvas.offsetWidth;
            canvas.height = canvas.offsetHeight;
            signaturePad = new SignaturePad(canvas);
        }
    }, 500);

    function clearSignature() {
        if(signaturePad) signaturePad.clear();
    }
    
    function submitSignature() {
        if (signaturePad.isEmpty()) {
            alert('Please provide a signature first.');
            return;
        }
        var base64Signature = signaturePad.toDataURL();
        
        // Hide modal and execute FSM with signature payload
        document.getElementById('signatureModal').style.display='none';
        
        fetch('/$area/Document/WorkflowAction', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({
                trackingNumber: '@Model.TrackingNumber',
                actionType: 'Approve',
                remarks: document.getElementById('wfRemarks').value,
                signatureData: base64Signature
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
</script>
"@
        if ($content -notmatch 'id="signatureModal"') {
            $content = $content -replace '</body>', "$signatureModal`n</body>"
            $content = $content -replace '</html>', "$signatureModal`n</html>" # in case it doesn't have body tag but it's a view, so it doesn't have body tag at all!
        }
        
        # It's a view, it doesn't have </body>. Let's just append it to the end.
        if ($content -notmatch 'id="signatureModal"') {
            $content = $content + "`n" + $signatureModal
        }

        # 2. Modify executeWorkflow to intercept 'Approve'
        $content = $content -replace 'if\(confirm\(''Are you sure you want to '' \+ action \+ '' this document\?''\)\)', "if(action === 'Approve') { document.getElementById('signatureModal').style.display='flex'; return; }`n            if(confirm('Are you sure you want to ' + action + ' this document?'))"

        Set-Content -Path $viewPath -Value $content
        Write-Host "Updated UI Actions for Signature in $area"
    }
}
