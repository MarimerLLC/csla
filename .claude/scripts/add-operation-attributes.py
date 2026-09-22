import re, subprocess, sys, collections

APPLY = "--apply" in sys.argv
OPS = {
  "DataPortal_Create": "Create", "DataPortal_Fetch": "Fetch", "DataPortal_Insert": "Insert",
  "DataPortal_Update": "Update", "DataPortal_Delete": "Delete", "DataPortal_DeleteSelf": "DeleteSelf",
  "DataPortal_Execute": "Execute",
  "Child_Create": "CreateChild", "Child_Fetch": "FetchChild", "Child_Insert": "InsertChild",
  "Child_Update": "UpdateChild", "Child_DeleteSelf": "DeleteSelfChild", "Child_Execute": "ExecuteChild",
}
ALL_ATTRS = set(OPS.values())
MODS = r"(?:public|private|protected|internal|static|async|virtual|override|new|sealed|unsafe|extern)"
DECL = re.compile(
  r"^(?P<indent>\s*)(?P<inline>(?:\[[^\]]*\]\s*)*)(?P<mods>(?:" + MODS + r"\s+)*)"
  r"(?P<ret>[\w.<>\[\],?]+(?:\s*<[^()]*>)?)\s+(?P<name>(?:DataPortal|Child)_(?:Create|Fetch|Insert|Update|DeleteSelf|Delete|Execute))(?P<async>Async)?\s*\(")
ATTR_NAME = re.compile(r"\[\s*([\w.]+)")

files = subprocess.run(["git", "grep", "-l", "-E",
  r"(DataPortal|Child)_(Create|Fetch|Insert|Update|Delete|DeleteSelf|Execute)(Async)?\b", "--"] + [a for a in sys.argv[1:] if a != "--apply"],
  capture_output=True, text=True).stdout.split()

def attrs_in(text):
  names = set()
  for part in re.findall(r"\[([^\]]*)\]", text):
    for a in part.split(","):
      a = a.strip().split("(")[0].strip()
      a = a.split(".")[-1]
      if a.endswith("Attribute"): a = a[:-9]
      names.add(a)
  return names

report = collections.defaultdict(list)
for f in files:
  raw = open(f, "rb").read()
  bom = raw.startswith(b"\xef\xbb\xbf")
  text = raw.decode("utf-8-sig")
  nl = "\r\n" if "\r\n" in text else "\n"
  lines = text.split(nl)
  inserts = []
  file_attr_ops = collections.Counter()
  decls = []
  for i, line in enumerate(lines):
    m = DECL.match(line)
    if not m or m.group("ret") in ("return", "await", "new", "throw", "base", "this"):
      continue
    if line.rstrip().endswith(";") and "=>" not in line:
      continue  # abstract/interface/call
    name = m.group("name")
    want = OPS[name]
    found = attrs_in(m.group("inline"))
    j = i - 1
    while j >= 0:
      s = lines[j].strip()
      if s.startswith("[") :
        found |= attrs_in(s); j -= 1; continue
      if s == "" or s.startswith("//") or s.startswith("#"):
        j -= 1; continue
      break
    ops = found & ALL_ATTRS
    decls.append((i, name, m, ops, want))
    for o in ops: file_attr_ops[o] += 1
  for i, name, m, ops, want in decls:
    mods = m.group("mods")
    tag = []
    if "override" in mods: tag.append("override")
    if "abstract" in lines[i]: tag.append("abstract")
    if ops:
      if want not in ops:
        report["MISMATCH"].append(f"{f}:{i+1}: {name} has {ops}")
      continue
    if file_attr_ops[want]:
      tag.append(f"file-already-has-[{want}]")
    report["ADD"].append(f"{f}:{i+1}: [{want}] {lines[i].strip()} {' '.join(tag)}")
    inserts.append((i, m.group("indent") + f"[{want}]"))
  if APPLY and inserts:
    for i, a in sorted(inserts, reverse=True):
      lines.insert(i, a)
    out = nl.join(lines).encode("utf-8")
    open(f, "wb").write((b"\xef\xbb\xbf" if bom else b"") + out)

for k in ("MISMATCH", "ADD"):
  print(f"== {k} ({len(report[k])})")
  for r in report[k]: print(r)
