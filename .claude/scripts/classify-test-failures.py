import re, sys, collections

log = open(sys.argv[1], encoding="utf-8", errors="replace").read()
# vstest normal verbosity: "  Failed Name [x ms]\n  Error Message:\n   ...\n  Stack Trace:"
blocks = re.findall(r"^\s+Failed (\S+).*?\n\s+Error Message:\n(.*?)\n\s+Stack Trace:", log, re.S | re.M)
cats = collections.defaultdict(list)
for name, msg in blocks:
    msg = " ".join(msg.split())
    m = re.search(r"TargetParameterCountException: ([\w.`]+)(?:\[\[.*?\]\])?\.\[(\w+)\]\(([^)]*)\)", msg)
    if m:
        # the base class chain tells where a name-matched CSLA method used to be found
        chain = re.findall(r"TargetParameterCountException: (Csla\.[\w.`]+)", msg)
        csla = [c for c in chain if not c.startswith("Csla.Test")]
        key = f"[{m.group(2)}]({m.group(3)}) on {m.group(1)}; first CSLA base: {csla[0] if csla else '-'}"
    else:
        key = "OTHER: " + msg[:220]
    cats[key].append(name)
for k, v in sorted(cats.items(), key=lambda kv: -len(kv[1])):
    print(f"{len(v):3} {k}\n      {', '.join(sorted(set(v)))[:400]}")
print("total failed blocks:", len(blocks))
