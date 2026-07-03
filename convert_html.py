with open('new_login.html', 'r', encoding='utf-8') as f:
    content = f.read()

print("Form tag exists:", "<form" in content)
print("Input email exists:", "type=\"email\"" in content or "type=\"text\"" in content)
print("Input password exists:", "type=\"password\"" in content)

# Check the input fields to see what classes or ids they have
import re
inputs = re.findall(r'<input[^>]+>', content)
for i in inputs:
    print(i)
