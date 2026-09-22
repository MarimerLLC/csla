import subprocess
files = subprocess.run(["git", "grep", "-l", "net11.0", "--", "Source/*.csproj", "Source/*.props", "Source/*.targets"],
                       capture_output=True, text=True).stdout.split()
changed = []
for p in files:
    b = open(p, "rb").read()
    n = b.replace(b";net11.0-windows", b"").replace(b";net11.0", b"").replace(b"net11.0;", b"")
    if n != b:
        open(p, "wb").write(n)
        changed.append(p)
print(len(files), len(changed))
for p in changed:
    print(p)
