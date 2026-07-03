import os

areas = ['Admin', 'Cart', 'DepartmentHead', 'Mayor']

buttons_html = '''
                        <button type="button" class="btn btn-outline" style="color: #F59E0B; border-color: #F59E0B;" onclick="openModal('returnModal')">
                            <i data-lucide="corner-up-left"></i> Return
                        </button>
                        <button type="button" class="btn btn-outline" style="color: #EF4444; border-color: #EF4444;" onclick="openModal('rejectModal')">
                            <i data-lucide="x-circle"></i> Reject
                        </button>
                        <button type="button" class="btn btn-outline" style="color: #3B82F6; border-color: #3B82F6;" onclick="openModal('reassignModal')">
                            <i data-lucide="users"></i> Reassign
                        </button>
'''

modals_html = '''
<div class="modal" id="returnModal">
    <div class="modal-dialog">
        <div class="modal-header">
            <h3 class="modal-title">Return Document</h3>
            <button type="button" class="btn btn-secondary btn-sm" onclick="closeModal('returnModal')"><i data-lucide="x"></i></button>
        </div>
        <div class="modal-body">
            <div class="form-group">
                <label>Remarks / Reason for Return</label>
                <textarea id="returnRemarks" class="form-control" rows="3" placeholder="Explain why the document is being returned..."></textarea>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" onclick="closeModal('returnModal')">Cancel</button>
            <button type="button" class="btn btn-primary" onclick="submitFsmAction('RETURN', document.getElementById('returnRemarks').value)">Confirm Return</button>
        </div>
    </div>
</div>

<div class="modal" id="rejectModal">
    <div class="modal-dialog">
        <div class="modal-header">
            <h3 class="modal-title">Reject Document</h3>
            <button type="button" class="btn btn-secondary btn-sm" onclick="closeModal('rejectModal')"><i data-lucide="x"></i></button>
        </div>
        <div class="modal-body">
            <div class="form-group">
                <label>Remarks / Reason for Rejection</label>
                <textarea id="rejectRemarks" class="form-control" rows="3" placeholder="Explain why the document is being rejected..."></textarea>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" onclick="closeModal('rejectModal')">Cancel</button>
            <button type="button" class="btn" style="background:#EF4444; color:white;" onclick="submitFsmAction('REJECT', document.getElementById('rejectRemarks').value)">Confirm Reject</button>
        </div>
    </div>
</div>

<div class="modal" id="reassignModal">
    <div class="modal-dialog">
        <div class="modal-header">
            <h3 class="modal-title">Reassign Document</h3>
            <button type="button" class="btn btn-secondary btn-sm" onclick="closeModal('reassignModal')"><i data-lucide="x"></i></button>
        </div>
        <div class="modal-body">
            <div class="form-group">
                <label>Target User ID</label>
                <input type="number" id="reassignUserId" class="form-control" placeholder="Enter User ID to reassign to..." />
            </div>
            <div class="form-group">
                <label>Remarks</label>
                <textarea id="reassignRemarks" class="form-control" rows="2" placeholder="Optional remarks..."></textarea>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" onclick="closeModal('reassignModal')">Cancel</button>
            <button type="button" class="btn btn-primary" onclick="submitFsmAction('REASSIGN', document.getElementById('reassignRemarks').value, document.getElementById('reassignUserId').value)">Confirm Reassign</button>
        </div>
    </div>
</div>

<script>
    function submitFsmAction(action, remarks, targetUserId = null) {
        const payload = {
            TrackingNumber: '@Model.TrackingNumber',
            Action: action,
            Remarks: remarks,
            TargetUserId: targetUserId ? parseInt(targetUserId) : null
        };
        
        fetch(window.location.pathname.replace('/Details/', '/Transition'), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                window.location.reload();
            } else {
                alert(data.message);
            }
        });
    }
</script>
'''

for area in areas:
    view_path = f'Areas/{area}/Views/Document/Details.cshtml'
    if not os.path.exists(view_path): continue
    
    with open(view_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    if 'returnModal' not in content:
        # Add buttons before the signature or endorse button
        if '<button type="button" class="btn btn-primary" onclick="openModal(\'signatureModal\')">' in content:
            content = content.replace('<button type="button" class="btn btn-primary" onclick="openModal(\'signatureModal\')">', buttons_html + '<button type="button" class="btn btn-primary" onclick="openModal(\'signatureModal\')">')
        elif '<button type="button" class="btn btn-primary" onclick="openModal(\'endorseModal\')">' in content:
            content = content.replace('<button type="button" class="btn btn-primary" onclick="openModal(\'endorseModal\')">', buttons_html + '<button type="button" class="btn btn-primary" onclick="openModal(\'endorseModal\')">')
            
        # Add modals at the bottom
        content += modals_html
        
        with open(view_path, 'w', encoding='utf-8') as f:
            f.write(content)
