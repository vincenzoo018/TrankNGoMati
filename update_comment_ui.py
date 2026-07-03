import os
import re

areas = ['Admin', 'Cart', 'DepartmentHead', 'Mayor', 'Receiving']

# Highlight @mentions in JS
js_mentions = '''
<script>
    document.addEventListener("DOMContentLoaded", function() {
        // Highlight mentions
        document.querySelectorAll('.comment-body').forEach(function(el) {
            let text = el.innerHTML;
            let highlighted = text.replace(/@([a-zA-Z0-9_]+)/g, '<span style="color:#3B82F6; font-weight:600; background:#EFF6FF; padding:2px 6px; border-radius:4px;">@$1</span>');
            el.innerHTML = highlighted;
        });
    });

    function showReplyForm(commentId) {
        document.getElementById('reply-form-' + commentId).style.display = 'block';
    }
    
    function hideReplyForm(commentId) {
        document.getElementById('reply-form-' + commentId).style.display = 'none';
    }
</script>
'''

for area in areas:
    view_path = f'Areas/{area}/Views/Document/Details.cshtml'
    if not os.path.exists(view_path): continue
    
    with open(view_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Rewrite the comment rendering loop to handle replies
    if 'Model.DocumentComments.Where(c => c.ParentCommentId == null)' not in content:
        # We need to find the foreach (var comment in Model.DocumentComments)
        # And replace it with a recursive or nested structure
        old_loop = '''@foreach (var comment in Model.DocumentComments.OrderByDescending(c => c.DateAdded))
                {
                    <div style="padding: 16px; border-bottom: 1px solid var(--border); display: flex; gap: 16px;">
                        <div style="width: 40px; height: 40px; border-radius: 50%; background-color: var(--brand); color: white; display: flex; align-items: center; justify-content: center; font-weight: 600; font-size: 16px; flex-shrink: 0;">
                            @comment.User.FullName.Substring(0, 1)
                        </div>
                        <div style="flex: 1;">
                            <div style="display: flex; justify-content: space-between; margin-bottom: 4px;">
                                <div>
                                    <span style="font-weight: 600; font-size: 14px;">@comment.User.FullName</span>
                                    <span style="color: var(--ink-muted); font-size: 13px; margin-left: 8px;">@comment.User.Department</span>
                                </div>
                                <span style="color: var(--ink-muted); font-size: 12px;">@comment.DateAdded.ToString("MMM dd, yyyy HH:mm")</span>
                            </div>
                            <div style="font-size: 14px; color: var(--ink); line-height: 1.5; white-space: pre-wrap;">@comment.Remarks</div>
                        </div>
                    </div>
                }'''

        new_loop = '''@foreach (var comment in Model.DocumentComments.Where(c => c.ParentCommentId == null).OrderBy(c => c.DateAdded))
                {
                    <div style="padding: 16px; border-bottom: 1px solid var(--border); display: flex; gap: 16px; flex-direction:column;">
                        <div style="display: flex; gap: 16px;">
                            <div style="width: 40px; height: 40px; border-radius: 50%; background-color: var(--brand); color: white; display: flex; align-items: center; justify-content: center; font-weight: 600; font-size: 16px; flex-shrink: 0;">
                                @comment.User.FullName.Substring(0, 1)
                            </div>
                            <div style="flex: 1;">
                                <div style="display: flex; justify-content: space-between; margin-bottom: 4px;">
                                    <div>
                                        <span style="font-weight: 600; font-size: 14px;">@comment.User.FullName</span>
                                        <span style="color: var(--ink-muted); font-size: 13px; margin-left: 8px;">@comment.User.Department</span>
                                    </div>
                                    <span style="color: var(--ink-muted); font-size: 12px;">@comment.DateAdded.ToString("MMM dd, yyyy HH:mm")</span>
                                </div>
                                <div class="comment-body" style="font-size: 14px; color: var(--ink); line-height: 1.5; white-space: pre-wrap;">@comment.Remarks</div>
                                <div style="margin-top: 8px;">
                                    <button class="btn btn-sm btn-outline" style="font-size:11px; padding:2px 8px;" onclick="showReplyForm(@comment.Id)">Reply</button>
                                </div>
                            </div>
                        </div>
                        
                        <!-- Replies -->
                        <div style="margin-left: 56px; display:flex; flex-direction:column; gap:16px;">
                            @foreach(var reply in Model.DocumentComments.Where(c => c.ParentCommentId == comment.Id).OrderBy(c => c.DateAdded))
                            {
                                <div style="display: flex; gap: 16px; background:#fafafa; padding:12px; border-radius:8px; border:1px solid var(--border);">
                                    <div style="width: 32px; height: 32px; border-radius: 50%; background-color: #64748b; color: white; display: flex; align-items: center; justify-content: center; font-weight: 600; font-size: 14px; flex-shrink: 0;">
                                        @reply.User.FullName.Substring(0, 1)
                                    </div>
                                    <div style="flex: 1;">
                                        <div style="display: flex; justify-content: space-between; margin-bottom: 4px;">
                                            <div>
                                                <span style="font-weight: 600; font-size: 13px;">@reply.User.FullName</span>
                                            </div>
                                            <span style="color: var(--ink-muted); font-size: 11px;">@reply.DateAdded.ToString("MMM dd, yyyy HH:mm")</span>
                                        </div>
                                        <div class="comment-body" style="font-size: 13px; color: var(--ink); line-height: 1.5; white-space: pre-wrap;">@reply.Remarks</div>
                                    </div>
                                </div>
                            }
                            
                            <!-- Reply Form -->
                            <div id="reply-form-@comment.Id" style="display:none; margin-top:8px;">
                                <form method="post" action="/@ViewContext.RouteData.Values["area"]/Document/Comment">
                                    <input type="hidden" name="trackingNumber" value="@Model.TrackingNumber" />
                                    <input type="hidden" name="parentCommentId" value="@comment.Id" />
                                    <div style="display: flex; gap: 12px;">
                                        <input type="text" name="remarks" class="form-control" placeholder="Type a reply... Use @username to mention" required style="flex: 1;" />
                                        <button type="submit" class="btn btn-primary btn-sm">Reply</button>
                                        <button type="button" class="btn btn-secondary btn-sm" onclick="hideReplyForm(@comment.Id)">Cancel</button>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                }'''
                
        content = content.replace(old_loop, new_loop)
        
        if 'showReplyForm' not in content:
            content += js_mentions
            
        with open(view_path, 'w', encoding='utf-8') as f:
            f.write(content)
