import base64
import os
import re

with open('new_login.html', 'r', encoding='utf-8') as f:
    content = f.read()

match = re.search(r'src="data:image/png;base64,([^"]+)', content)
if match:
    b64_str = match.group(1)
    print('Found base64 string of length:', len(b64_str))
    try:
        # Pad if needed
        b64_str += "=" * ((4 - len(b64_str) % 4) % 4)
        data = base64.b64decode(b64_str)
        os.makedirs('wwwroot/images', exist_ok=True)
        with open('wwwroot/images/mati_seal.png', 'wb') as img_f:
            img_f.write(data)
        print('Saved successfully to wwwroot/images/mati_seal.png')
    except Exception as e:
        print('Failed to decode:', e)
else:
    print('No base64 string found')
