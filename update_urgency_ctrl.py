import os

ctrl_path = r'Areas\Receiving\Controllers\DocumentController.cs'
with open(ctrl_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Update method signature
if 'bool IsUrgent = false' not in content:
    content = content.replace('IFormFile? AttachedDocument)', 'IFormFile? AttachedDocument, bool IsUrgent = false, string? UrgencyJustification = null)')

# Find the new Document instantiation block and add fields
target_doc = '''            var doc = new Document
            {
                TrackingNumber        = trackingNumber,
                Title                 = Title,
                DocumentType          = type.TypeName,
                TypeId                = type.Id,
                OriginatingDepartment = dept.DepartmentName,
                DepartmentId          = dept.Id,'''

replacement_doc = '''            var doc = new Document
            {
                TrackingNumber        = trackingNumber,
                Title                 = Title,
                DocumentType          = type.TypeName,
                TypeId                = type.Id,
                OriginatingDepartment = dept.DepartmentName,
                DepartmentId          = dept.Id,
                IsUrgent              = IsUrgent,
                UrgencyJustification  = UrgencyJustification,'''
                
content = content.replace(target_doc, replacement_doc)

with open(ctrl_path, 'w', encoding='utf-8') as f:
    f.write(content)
