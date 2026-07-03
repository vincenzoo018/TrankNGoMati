import os

view_path = r'Areas\Receiving\Views\Document\Index.cshtml'
if os.path.exists(view_path):
    with open(view_path, 'r', encoding='utf-8') as f:
        content = f.read()

    template_html = '''
                  <!-- Template Selection -->
                  <div class="form-row" style="margin-bottom: 16px;">
                      <div class="form-group" style="flex:1;">
                          <label class="form-label">Use a Pre-defined Template</label>
                          <select class="form-control" id="templateSelect" onchange="applyTemplate(this)">
                              <option value="">-- Start from Blank --</option>
                              @if(ViewBag.Templates != null) {
                                  foreach(var t in ViewBag.Templates) {
                                      <option value="@t.Title">@t.Title (@t.Category)</option>
                                  }
                              }
                          </select>
                      </div>
                  </div>
'''

    js_html = '''
          function applyTemplate(selectElem) {
              if (selectElem.value) {
                  document.getElementById('reg-title').value = selectElem.value;
              }
          }
'''

    if 'id="templateSelect"' not in content:
        # Insert before Title input
        target = '<div class="form-group" style="flex:2;">'
        if target in content:
            content = content.replace(target, template_html + target)
            
        # Insert JS function
        target_js = 'function useOcrData() {'
        if target_js in content:
            content = content.replace(target_js, js_html + target_js)
            
        with open(view_path, 'w', encoding='utf-8') as f:
            f.write(content)
