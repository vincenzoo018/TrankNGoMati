$areas = @("Receiving", "Admin", "Mayor", "DepartmentHead", "Cart")

foreach ($area in $areas) {
    $ctrlPath = "c:\Users\Huawei\source\repos\TrankNGoMati\Areas\$area\Controllers\DocumentController.cs"
    if (Test-Path $ctrlPath) {
        $content = Get-Content -Path $ctrlPath -Raw

        # 1. Inject WorkflowEngine dependency if not present
        if ($content -notmatch 'private readonly WorkflowEngine _workflow;') {
            $content = $content -replace 'private readonly IWebHostEnvironment _env;', "private readonly IWebHostEnvironment _env;`n        private readonly WorkflowEngine _workflow;"
            $content = $content -replace 'public DocumentController\((.*?)IWebHostEnvironment env\)', 'public DocumentController($1IWebHostEnvironment env, WorkflowEngine workflow)'
            $content = $content -replace '_env     = env;', "_env     = env;`n            _workflow = workflow;"
        }

        # 2. Add Return, Reject, Reassign endpoints (and replace Forward)
        $newEndpoints = @"
        [HttpPost]
        public IActionResult WorkflowAction(string trackingNumber, string actionType, string remarks, int? targetUserId)
        {
            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;
            bool success = _workflow.TransitionDocument(trackingNumber, userId, actionType, remarks, targetUserId);
            
            if (success) {
                // If SMS is needed, we trigger it based on state (mocked for now)
                return Json(new { success = true, redirectUrl = "/$area/Document" });
            }
            return Json(new { success = false, message = "Invalid transition or insufficient permissions." });
        }
"@
        
        # Replace the entire Forward block with WorkflowAction, or just append WorkflowAction before AddAnchoredComment
        if ($content -notmatch 'public IActionResult WorkflowAction') {
            $content = $content -replace '\[HttpPost\]\s*public IActionResult Forward.*?(?=\[HttpPost\]\s*public IActionResult AddAnchoredComment)', "$newEndpoints`n        "
        }

        Set-Content -Path $ctrlPath -Value $content
        Write-Host "Updated DocumentController for $area"
    }
}
