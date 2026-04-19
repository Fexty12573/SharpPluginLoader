import sys
import os
import shutil
from pathlib import Path
import subprocess
import hashlib

# Use a Developer Command Prompt/Powershell for VS 20XX to get dumpbin.exe in your PATH.

if len(sys.argv) < 2:
    print(f'Usage: {sys.argv[0]} [path_to_dll] (optional_path_to_dumpbin.exe)')
    exit(1)

if len(sys.argv) == 2:
    dumpbin = shutil.which('dumpbin.exe')
else:
    dumpbin = sys.argv[2]
if not dumpbin or not os.path.isfile(dumpbin):
    print(f'Error: dumpbin.exe not found' + (' in PATH' if dumpbin is None else f' at \'{dumpbin}\''))
    exit(1)

dll = Path(sys.argv[1])
if not dll.exists():
    print(f'Error: dll not found at \'{dll}\'')
    exit(1)
dllbase = dll.parts[-1]

if os.name == 'nt': # Windows.
    version_cmd = ('wmic', 'datafile', 'where', 'name = "{:s}"'.format(str(dll.absolute()).replace('\\', '\\\\')), 'get', 'Version', '/value')
    output = subprocess.run(version_cmd, stdout=subprocess.PIPE, shell=True)
    dll_version = output.stdout.decode('utf-8').strip().replace('=', ' ')
    output = subprocess.run([dumpbin, '/EXPORTS', '/NOLOGO', dll], stdout=subprocess.PIPE)
else: # Try to use Wine.
    dll_version = 'unknown'
    os.environ['WINEDEBUG'] = '-all'
    output = subprocess.run(['wine', dumpbin, '/EXPORTS', '/NOLOGO', dll], stdout=subprocess.PIPE)

m = hashlib.sha256()
with open(dll, 'rb') as f:
    m.update(f.read())

print(f'// Exports for {dllbase} ({dll_version}, sha256-{m.hexdigest()})')
print(f'#pragma region {dllbase} export forwarding')
lines = output.stdout.decode('utf-8').splitlines()
while not lines[0].startswith('    ordinal hint'):
    lines.pop(0)
for l in lines[2:]:
    if len(l) == 0:
        break
    last_space = l.rfind(' ')
    if last_space >= 0:
        l = l[last_space + 1:]
    if l == '[NONAME]':
        break
    print(f'#pragma comment(linker, "/export:{l}=\\\"C:\\\\Windows\\\\System32\\\\{dll.stem}.{l}\\\"")')
print('#pragma endregion')
