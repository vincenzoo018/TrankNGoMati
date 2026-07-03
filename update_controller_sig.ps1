$files = Get-ChildItem -Path .\Areas\*\Controllers\DocumentController.cs
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    $content = $content -replace "public IActionResult WorkflowAction\(string trackingNumber, string actionType, string remarks, int\? targetUserId\)", "public IActionResult WorkflowAction(string trackingNumber, string actionType, string remarks, int? targetUserId, string signatureData = null)"
    $content = $content -replace "_workflow\.TransitionDocument\(trackingNumber, userId, actionType, remarks, targetUserId\)", "_workflow.TransitionDocument(trackingNumber, userId, actionType, remarks, targetUserId, signatureData)"
    Set-Content -Path $f.FullName -Value $content
}
