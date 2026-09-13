#!/usr/bin/env python3
"""
Find and unlist old prerelease package versions on nuget.org.

A listed prerelease version is unlisted when:
  - the same package has a listed stable version whose core version is equal
    or higher (10.0.0-beta-0022 goes once 10.0.0 ships; 10.2.0-beta.1 stays
    until 10.2.0 or later is published), or
  - its only prerelease tag is a git commit id (e.g. 10.0.1-geeb45f365a),
    meaning a non-public build was pushed by mistake, or
  - its package id was passed with --include, or --all-prerelease was given.

Dry run by default: prints the plan and changes nothing.
See delist-prerelease-packages.md for full usage.
"""

import argparse
import json
import os
import re
import sys
import time
import urllib.error
import urllib.parse
import urllib.request

SEARCH_URL = "https://azuresearch-usnc.nuget.org/query"
DELETE_URL = "https://www.nuget.org/api/v2/package/{id}/{version}"
# Nerdbank.GitVersioning appends only "-g<commit>" when PublicRelease isn't set,
# so a version like 10.0.1-geeb45f365a is a build that was never meant to ship.
GIT_COMMIT_TAG = re.compile(r"g[0-9a-f]{7,40}")


def core_version(version):
  core = version.split("+", 1)[0].split("-", 1)[0]
  parts = [int(p) if p.isdigit() else 0 for p in core.split(".")]
  return tuple((parts + [0, 0, 0, 0])[:4])


def owned_packages(owner):
  query = urllib.parse.urlencode({
    "q": f"owner:{owner}",
    "prerelease": "true",
    "semVerLevel": "2.0.0",
    "take": 1000,
  })
  with urllib.request.urlopen(f"{SEARCH_URL}?{query}") as response:
    data = json.load(response)
  # The search index only returns listed versions, which is exactly what we need.
  return [p for p in data["data"] if owner.lower() in (o.lower() for o in p.get("owners", []))]


def prerelease_tag(version):
  release = version.split("+", 1)[0]
  return release.split("-", 1)[1] if "-" in release else None


def plan(packages, prefix, all_prerelease, include):
  include = {i.lower() for i in include}
  result = []
  for package in sorted(packages, key=lambda p: p["id"].lower()):
    if not package["id"].lower().startswith(prefix.lower()):
      continue
    versions = [v["version"] for v in package["versions"]]
    stable = [core_version(v) for v in versions if prerelease_tag(v) is None]
    newest_stable = max(stable) if stable else None
    for version in versions:
      tag = prerelease_tag(version)
      if tag is None:
        continue
      if newest_stable is not None and newest_stable >= core_version(version):
        reason = "superseded by stable"
      elif GIT_COMMIT_TAG.fullmatch(tag):
        reason = "non-public build"
      elif package["id"].lower() in include:
        reason = "--include"
      elif all_prerelease:
        reason = "--all-prerelease"
      else:
        continue
      result.append((package["id"], version, reason))
  return result


def unlist(package_id, version, api_key):
  url = DELETE_URL.format(id=urllib.parse.quote(package_id), version=urllib.parse.quote(version))
  request = urllib.request.Request(url, method="DELETE", headers={
    "X-NuGet-ApiKey": api_key,
    "User-Agent": "csla-delist-prerelease",
  })
  with urllib.request.urlopen(request) as response:
    return response.status


def main():
  parser = argparse.ArgumentParser(description="Unlist superseded prerelease packages on nuget.org")
  parser.add_argument("--owner", default="rockfordlhotka", help="nuget.org owner profile name")
  parser.add_argument("--prefix", default="Csla", help="only packages whose id starts with this")
  parser.add_argument("--all-prerelease", action="store_true", help="also unlist prereleases with no newer stable release")
  parser.add_argument("--include", action="append", default=[], metavar="ID",
                      help="also unlist every prerelease of this package id (repeatable)")
  parser.add_argument("--apply", action="store_true", help="actually unlist (requires NUGET_API_KEY env var)")
  args = parser.parse_args()

  targets = plan(owned_packages(args.owner), args.prefix, args.all_prerelease, args.include)
  if not targets:
    print("Nothing to unlist.")
    return 0

  id_width = max(len(i) for i, _, _ in targets)
  version_width = max(len(v) for _, v, _ in targets)
  for package_id, version, reason in targets:
    print(f"  {package_id:<{id_width}}  {version:<{version_width}}  {reason}")
  print(f"\n{len(targets)} prerelease version(s) {'to unlist' if args.apply else 'would be unlisted (dry run)'}.")

  if not args.apply:
    return 0

  api_key = os.environ.get("NUGET_API_KEY")
  if not api_key:
    print("Error: set NUGET_API_KEY to an API key with the 'Unlist package' scope.", file=sys.stderr)
    return 1

  failures = 0
  for package_id, version, _ in targets:
    try:
      status = unlist(package_id, version, api_key)
      print(f"  unlisted {package_id} {version} ({status})")
    except urllib.error.HTTPError as ex:
      failures += 1
      print(f"  FAILED   {package_id} {version} ({ex.code} {ex.reason})", file=sys.stderr)
      if ex.code in (401, 403) and failures == 1:
        print("  (check the key's scope and glob pattern)", file=sys.stderr)
    time.sleep(0.5)

  print(f"\nDone: {len(targets) - failures} unlisted, {failures} failed.")
  return 1 if failures else 0


if __name__ == "__main__":
  sys.exit(main())
