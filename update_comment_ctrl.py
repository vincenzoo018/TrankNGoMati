import os
areas = ['Admin', 'Cart', 'DepartmentHead', 'Mayor', 'Receiving']
for area in areas:
    ctrl_path = f'Areas/{area}/Controllers/DocumentController.cs'
    if not os.path.exists(ctrl_path): continue
    
    with open(ctrl_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    if 'public IActionResult Comment(string trackingNumber, string remarks)' in content:
        content = content.replace('public IActionResult Comment(string trackingNumber, string remarks)', 'public IActionResult Comment(string trackingNumber, string remarks, int? parentCommentId = null)')
        
        target = 'new DocumentComment { DocumentId = doc.Id, UserId = userId, Remarks = remarks, DateAdded = DateTime.Now, RemarkType = "Comment", Resolved = false }'
        replacement = 'new DocumentComment { DocumentId = doc.Id, UserId = userId, Remarks = remarks, DateAdded = DateTime.Now, RemarkType = "Comment", Resolved = false, ParentCommentId = parentCommentId }'
        content = content.replace(target, replacement)
        
        with open(ctrl_path, 'w', encoding='utf-8') as f:
            f.write(content)
