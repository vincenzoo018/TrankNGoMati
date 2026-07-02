$areas = @(
    @{ Name="Receiving"; Step=1 },
    @{ Name="DepartmentHead"; Step=2 },
    @{ Name="Cart"; Step=3 },
    @{ Name="Mayor"; Step=4 },
    @{ Name="Admin"; Step=-1 }
)

foreach ($area in $areas) {
    $areaName = $area.Name
    $areaStep = $area.Step
    $ctrlPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$areaName\Controllers\DashboardController.cs"
    
    if (Test-Path $ctrlPath) {
        $ctrlContent = Get-Content -Path $ctrlPath -Raw
        
        # Replace the Index method body
        $newIndexMethod = @"
        public IActionResult Index()
        {
            ViewData["Title"] = "$areaName Dashboard";
            ViewBag.CurrentPage = "Dashboard";

            var docs = _context.Documents.ToList();
            
            ViewBag.TotalDocs = docs.Count;
            ViewBag.PendingDocs = docs.Count(d => d.CurrentStatus == 1 || d.CurrentStatus == 2);
            ViewBag.CompletedDocs = docs.Count(d => d.CurrentStatus == 3);
            
            var today = System.DateTime.Now;
            var overdue = docs.Where(d => d.CurrentStatus != 3 && (today - d.DateFiled).TotalDays > d.ArtaprocessingDays).ToList();
            ViewBag.OverdueDocs = overdue;

            if ($areaStep > 0) {
                ViewBag.ActionNeeded = docs.Where(d => d.CurrentStepIndex == $areaStep && d.CurrentStatus != 3).ToList();
            } else {
                ViewBag.ActionNeeded = new List<Document>(); // Admin sees all or none
            }

            // Funnel data
            ViewBag.Step1Count = docs.Count(d => d.CurrentStepIndex == 1 && d.CurrentStatus != 3);
            ViewBag.Step2Count = docs.Count(d => d.CurrentStepIndex == 2 && d.CurrentStatus != 3);
            ViewBag.Step3Count = docs.Count(d => d.CurrentStepIndex == 3 && d.CurrentStatus != 3);
            ViewBag.Step4Count = docs.Count(d => d.CurrentStepIndex == 4 && d.CurrentStatus != 3);

            return View();
        }
"@
        $ctrlContent = $ctrlContent -replace '(?s)public IActionResult Index\(\)\s*\{.*?\n        \}', $newIndexMethod
        Set-Content -Path $ctrlPath -Value $ctrlContent
        Write-Host "Updated DashboardController for $areaName"
    }

    $viewPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$areaName\Views\Dashboard\Index.cshtml"
    if (Test-Path $viewPath) {
        $viewContent = Get-Content -Path $viewPath -Raw
        
        # Insert Overdue widget at the top (after page header)
        $overdueWidget = @"
</div>

@if (ViewBag.OverdueDocs != null && ((IEnumerable<TrackNGoMati.Models.Document>)ViewBag.OverdueDocs).Any())
{
    <div class="animate-fade-in" style="background-color: #FEF2F2; border: 1px solid #FCA5A5; border-radius: 8px; padding: 16px; margin-bottom: 24px; display: flex; align-items: center; gap: 16px;">
        <div style="background: #FEE2E2; color: #DC2626; width: 40px; height: 40px; border-radius: 50%; display: flex; align-items: center; justify-content: center;">
            <i data-lucide="alert-triangle"></i>
        </div>
        <div style="flex: 1;">
            <h4 style="margin: 0; color: #991B1B; font-size: 14px; font-weight: 700;">SLA Breach Alert</h4>
            <p style="margin: 0; color: #B91C1C; font-size: 12px; margin-top: 4px;">There are <strong>@(((IEnumerable<TrackNGoMati.Models.Document>)ViewBag.OverdueDocs).Count())</strong> documents that have exceeded their ARTA processing SLA.</p>
        </div>
        <a href="/$areaName/Document" class="btn" style="background: white; border: 1px solid #FCA5A5; color: #DC2626;">View Overdue</a>
    </div>
}

@if (ViewBag.ActionNeeded != null && ((IEnumerable<TrackNGoMati.Models.Document>)ViewBag.ActionNeeded).Any())
{
    <div class="animate-fade-in" style="background-color: #F0FDF4; border: 1px solid #86EFAC; border-radius: 8px; padding: 16px; margin-bottom: 24px; display: flex; align-items: center; gap: 16px;">
        <div style="background: #DCFCE7; color: #16A34A; width: 40px; height: 40px; border-radius: 50%; display: flex; align-items: center; justify-content: center;">
            <i data-lucide="inbox"></i>
        </div>
        <div style="flex: 1;">
            <h4 style="margin: 0; color: #166534; font-size: 14px; font-weight: 700;">Action Required</h4>
            <p style="margin: 0; color: #15803D; font-size: 12px; margin-top: 4px;">You have <strong>@(((IEnumerable<TrackNGoMati.Models.Document>)ViewBag.ActionNeeded).Count())</strong> documents pending your review and signature.</p>
        </div>
        <a href="/$areaName/Document" class="btn" style="background: white; border: 1px solid #86EFAC; color: #16A34A;">Go to Inbox</a>
    </div>
}
"@
        $viewContent = $viewContent -replace '(?s)</div>\s*<div class="filter-bar', "$overdueWidget`n<div class=`"filter-bar"
        
        # Replace the Bar chart with Funnel Chart
        $funnelChartScript = @"
            var ctxBar = document.getElementById('barChart').getContext('2d');
            new Chart(ctxBar, {
                type: 'bar',
                data: {
                    labels: ['Receiving', 'Dept Head', 'CART', 'Mayor'],
                    datasets: [{
                        label: 'Documents Stuck',
                        data: [@ViewBag.Step1Count, @ViewBag.Step2Count, @ViewBag.Step3Count, @ViewBag.Step4Count],
                        backgroundColor: ['#1A56DB', '#3B82F6', '#60A5FA', '#93C5FD'],
                        borderRadius: 4
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { grid: { display: false }, border: { display: false } },
                        x: { beginAtZero: true, grid: { display: false }, border: { display: false } }
                    }
                }
            });
"@
        $viewContent = $viewContent -replace '(?s)var ctxBar = document.getElementById\(''barChart''\).getContext\(''2d''\);.*?options: \{.*?\}\s*\}\s*\);\s*\}\s*\);', "$funnelChartScript`n        });"
        $viewContent = $viewContent -replace 'Documents Filed Per Week — May 2026', 'Active Bottlenecks (Documents per Stage)'

        Set-Content -Path $viewPath -Value $viewContent
        Write-Host "Updated Dashboard Index for $areaName"
    }
}
