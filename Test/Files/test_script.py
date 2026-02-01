import sys
import os
import random
import traceback
import time

# stdin
tag = input()

# stdout
start_ms = time.time() * 1000.0
for i in range(5):
    now_ms = time.time() * 1000.0
    print(f'[{i} {now_ms - start_ms}] print {i} -> {tag}')

# stderr
print("Error message 1!!!", file=sys.stderr)
print("Error message 2!!!", file=sys.stderr)

sys.exit(999)
