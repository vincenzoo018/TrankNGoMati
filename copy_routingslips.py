import os, sys

areas = {
    'Admin': 'ROLE_ADMIN',
    'Cart': 'ROLE_CART',
    'DepartmentHead': 'ROLE_DEPT',
    'Mayor': 'ROLE_MAYOR'
}

with open('Areas/Receiving/Controllers/RoutingSlipController.cs', 'r') as f:
    ctrl_tmpl = f.read()

with open('Areas/Receiving/Views/RoutingSlip/Index.cshtml', 'r') as f:
    view_tmpl = f.read()

for area, role in areas.items():
    # Controller
    c = ctrl_tmpl.replace('Area("Receiving")', f'Area("{area}")')
    c = c.replace('namespace TrackNGoMati.Areas.Receiving.Controllers', f'namespace TrackNGoMati.Areas.{area}.Controllers')
    c = c.replace('ROLE_RECORDS', role)
    
    ctrl_dir = f'Areas/{area}/Controllers'
    os.makedirs(ctrl_dir, exist_ok=True)
    with open(f'{ctrl_dir}/RoutingSlipController.cs', 'w') as f:
        f.write(c)
        
    # View
    v = view_tmpl.replace('/Receiving/Document/Details/', f'/{area}/Document/Details/')
    view_dir = f'Areas/{area}/Views/RoutingSlip'
    os.makedirs(view_dir, exist_ok=True)
    with open(f'{view_dir}/Index.cshtml', 'w') as f:
        f.write(v)
    
    print(f'Processed {area}')
