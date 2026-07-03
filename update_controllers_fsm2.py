import os

areas = ['Admin', 'Cart', 'DepartmentHead', 'Mayor']

transition_method = '''
        [HttpPost]
        public IActionResult Transition([FromBody] TransitionRequest req)
        {
            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;
            // WorkflowEngine dependency is injected
            var workflowEngine = new TrackNGoMati.Services.WorkflowEngine(_context, null);
            bool success = workflowEngine.TransitionDocument(req.TrackingNumber, userId, req.Action, req.Remarks, req.TargetUserId, req.SignatureData);
            if (success)
            {
                return Json(new { success = true, message = $"Document successfully processed ({req.Action})." });
            }
            return Json(new { success = false, message = "Transition failed. You may not have permission or the document state is invalid." });
        }
        
        public class TransitionRequest {
            public string TrackingNumber { get; set; }
            public string Action { get; set; }
            public string Remarks { get; set; }
            public int? TargetUserId { get; set; }
            public string SignatureData { get; set; }
        }
'''

for area in areas:
    ctrl_path = f'Areas/{area}/Controllers/DocumentController.cs'
    if not os.path.exists(ctrl_path): continue
    
    with open(ctrl_path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    if 'public IActionResult Transition(' not in content:
        # Insert before the last closing brace
        last_brace = content.rfind('}')
        if last_brace != -1:
            last_brace = content.rfind('}', 0, last_brace)
            content = content[:last_brace] + transition_method + content[last_brace:]
            
    with open(ctrl_path, 'w', encoding='utf-8') as f:
        f.write(content)
