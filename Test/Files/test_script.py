import sys
import os
import random
import traceback
import time

# A script for hacking on, that's all.

start_ms = time.time() * 1000.0

for i in range(5):
    now_ms = time.time() * 1000.0
    print(f'[{i} {now_ms - start_ms}] print {i}')

sys.exit(999)
