import re

with open('C:/Users/daryl/OneDrive/Desktop/budget-api/docs/generate_pdf.py', 'r', encoding='utf-8') as f:
    content = f.read()

subs = {
    0x2022: '-', 0x2014: '--', 0x2013: '-', 0x00b7: '.',
    0x2019: "'", 0x201c: '"', 0x201d: '"',
    0x2264: '<=', 0x2265: '>=', 0x2500: '-', 0x2550: '=',
}

# Match \uXXXX escape sequences literally in source
# We need to find backslash + u + 4 hex digits
i = 0
result = []
while i < len(content):
    if content[i] == '\\' and i + 5 < len(content) and content[i+1] == 'u':
        hex_part = content[i+2:i+6]
        if all(c in '0123456789abcdefABCDEF' for c in hex_part):
            cp = int(hex_part, 16)
            if cp > 255:
                result.append(subs.get(cp, '?'))
                i += 6
                continue
    result.append(content[i])
    i += 1

content = ''.join(result)

with open('C:/Users/daryl/OneDrive/Desktop/budget-api/docs/generate_pdf.py', 'w', encoding='utf-8') as f:
    f.write(content)
print('done')
