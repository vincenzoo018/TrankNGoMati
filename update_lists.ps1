$areas = @("Receiving", "Admin", "Mayor", "DepartmentHead", "Cart")

foreach ($area in $areas) {
    $viewPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$area\Views\Document\Index.cshtml"
    if (Test-Path $viewPath) {
        $content = Get-Content -Path $viewPath -Raw
        
        # 1. Add Select All Checkbox to theader
        $thMatch = '<tr>\s*<th style="padding-left: 20px;">REF NO\.</th>'
        $thReplace = "<tr>`n                    <th style=`"padding-left: 20px; width: 40px;`"><input type=`"checkbox`" id=`"selectAll`" /></th>`n                    <th>REF NO.</th>"
        if ($content -notmatch 'id="selectAll"') {
            $content = $content -replace $thMatch, $thReplace
        }

        # 2. Add Row Checkbox
        $tdMatch = '<tr>\s*<td style="padding-left: 20px;"><a href="/[^"]+" style="font-weight:600'
        $tdReplace = "<tr>`n                        <td style=`"padding-left: 20px;`"><input type=`"checkbox`" class=`"bulk-cb`" value=`"@doc.TrackingNumber`" /></td>`n                        <td><a href=`"/$area/Document/Details/@doc.TrackingNumber`" style=`"font-weight:600"
        if ($content -notmatch 'class="bulk-cb"') {
            $content = $content -replace $tdMatch, $tdReplace
        }
        
        # 3. Add Bulk Actions dropdown in Filter bar
        $bulkActionBtn = '        <select class="filter-select" id="bulkAction" style="min-width: 150px; background:#EFF6FF; color:#1D4ED8; border-color:#BFDBFE; display:none;"><option value="">Bulk Actions</option><option value="forward">Forward Selected</option><option value="archive">Archive Selected</option></select>'
        if ($content -notmatch 'id="bulkAction"') {
            $content = $content -replace '<div class="filter-bar".*?>', "$&`n$bulkActionBtn"
        }

        # 4. Add Empty State CTA
        $emptyState = "<div style=`"margin-top:16px;`"><a href=`"/$area/Document/Create`" class=`"btn btn-primary`"><i data-lucide=`"plus`"></i> Submit New Document</a></div>"
        if ($content -notmatch 'Submit New Document') {
            $content = $content -replace 'No documents registered yet\.', "No documents found. $emptyState"
        }

        # 5. Add Sorting JS
        $js = @"
<script>
    document.getElementById('selectAll')?.addEventListener('change', function(e) {
        document.querySelectorAll('.bulk-cb').forEach(cb => cb.checked = e.target.checked);
        toggleBulk();
    });
    document.querySelectorAll('.bulk-cb').forEach(cb => cb.addEventListener('change', toggleBulk));
    
    function toggleBulk() {
        const anyChecked = document.querySelectorAll('.bulk-cb:checked').length > 0;
        document.getElementById('bulkAction').style.display = anyChecked ? 'inline-block' : 'none';
    }
</script>
"@
        if ($content -notmatch 'toggleBulk\(\)') {
            $content = $content -replace '</tbody>', "</tbody>`n$js"
        }

        Set-Content -Path $viewPath -Value $content
        Write-Host "Updated List View for $area"
    }
}
