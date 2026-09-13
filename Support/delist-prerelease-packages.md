# Unlisting old prerelease packages on nuget.org

`delist-prerelease-packages.py` finds old alpha, beta, and other prerelease versions of packages you own on nuget.org and unlists them.

Unlisting hides a version from search and from the NuGet UI's version picker. Projects that already reference that exact version can still restore it. Unlisting can be undone: open the package's **Manage** page on nuget.org and relist the version. nuget.org doesn't let you permanently delete packages.

## Prerequisites

- Python 3 (standard library only, nothing to install)
- A nuget.org API key with the **Unlist package** scope (only needed with `--apply`)

## Quick start

From Git Bash:

```bash
cd /s/src/rdl/csla/Support

# 1. See what would be unlisted (dry run, changes nothing)
python delist-prerelease-packages.py --include Csla.Web

# 2. Unlist those versions
NUGET_API_KEY=oy2... python delist-prerelease-packages.py --include Csla.Web --apply
```

From PowerShell:

```powershell
$env:NUGET_API_KEY = 'oy2...'
python .\delist-prerelease-packages.py --include Csla.Web --apply
Remove-Item Env:\NUGET_API_KEY
```

Always run the dry run first and read the list.

## Getting an API key

nuget.org still supports API keys, but each one expires after at most 365 days. The CI release workflow uses **trusted publishing** instead. That gives the workflow a key that lasts one hour and is meant for pushing packages, so it's no help for unlisting from your machine. Create a separate short-lived key for each cleanup:

1. Sign in at https://www.nuget.org and go to **API Keys** (https://www.nuget.org/account/apikeys).
2. Click **Create** and set:
   - **Key name:** something like `unlist-prerelease-2026-09`
   - **Expires in:** 1 day
   - **Select scopes:** **Unlist package** only (leave both Push options unchecked)
   - **Select packages → Glob pattern:** `Csla*` (or whatever prefix you're cleaning up)
3. Copy the key. nuget.org shows it only once.
4. After the run, delete the key on the same page.

## What gets unlisted

The script queries the nuget.org search index for every package owned by `--owner` whose id starts with `--prefix`. The search index only includes listed versions, so versions that are already unlisted are skipped automatically.

It unlists a prerelease version for any of these reasons (shown in the last column of the output):

| Reason | Rule | Example |
|---|---|---|
| `superseded by stable` | The package has a listed stable version with an equal or higher `major.minor.patch` | `10.0.0-beta-0022-gbd53196668` is unlisted once `10.0.0` is on nuget.org |
| `non-public build` | The only prerelease tag is a git commit id (`-g<hex>`). Nerdbank.GitVersioning adds that when a build wasn't marked `PublicRelease=true`, so it was pushed by mistake | `Csla.Maui 10.0.1-geeb45f365a` |
| `--include` | The package id was passed with `--include`. Every prerelease of that package is unlisted, even with no stable release | `Csla.Web 10.0.0-beta-0011-gc72d4fb04f` (package replaced by `Csla.Web.Mvc`) |
| `--all-prerelease` | The flag was given. Every prerelease is unlisted, **including the current beta** | `10.2.0-beta.1` |

Stable versions are never touched.

Prereleases for an upcoming version stay listed until that version ships (for example, `10.2.0-beta.1` stays while no stable 10.2.0 exists). Run the script again after each stable release to clear out the betas for that version.

## Options

| Option | Default | Description |
|---|---|---|
| `--owner OWNER` | `rockfordlhotka` | nuget.org profile name that owns the packages |
| `--prefix PREFIX` | `Csla` | Only consider package ids that start with this (not case-sensitive) |
| `--include ID` | none | Also unlist every prerelease of this package. Repeat for more than one package |
| `--all-prerelease` | off | Unlist every listed prerelease, including current betas |
| `--apply` | off | Actually unlist. Without it the script only prints the plan |

The API key is read from the `NUGET_API_KEY` environment variable, so it doesn't end up in your shell history as an argument. Don't commit a key to this script or anywhere else in the repo.

## Examples

```bash
# Routine cleanup after a stable release
python delist-prerelease-packages.py
NUGET_API_KEY=... python delist-prerelease-packages.py --apply

# Retire every prerelease of packages that are no longer maintained
python delist-prerelease-packages.py --include Csla.Web --include Csla.Uwp

# Check other packages you own
python delist-prerelease-packages.py --prefix RockBot

# Wipe every prerelease, current betas included
python delist-prerelease-packages.py --all-prerelease
```

## Output

Dry run:

```
  Csla       10.0.0-beta-0022-gbd53196668   superseded by stable
  Csla.Maui  10.0.1-geeb45f365a             non-public build
  Csla.Web   10.0.0-beta-0011-gc72d4fb04f   --include

87 prerelease version(s) would be unlisted (dry run).
```

With `--apply`, each version prints `unlisted` or `FAILED` along with the HTTP status. The script ends with a summary and exits with code 1 if anything failed.

## Troubleshooting

- **403 Forbidden:** the key is missing the **Unlist package** scope, its glob pattern doesn't match the package id, or the key has expired.
- **401 Unauthorized:** the key is wrong or has been deleted.
- **A version still appears after unlisting:** the search index updates a few minutes after the change. Run the dry run again later to confirm.
- **An unlisted version shows up in the plan:** this is the same search-index delay. Unlisting a version twice does no harm.
- **Need to undo:** on nuget.org, go to the package → **Manage package** → **Listing**, then check the version to relist it.

## How it works

- Package discovery: `GET https://azuresearch-usnc.nuget.org/query?q=owner:<owner>&prerelease=true&semVerLevel=2.0.0`
- Unlist: `DELETE https://www.nuget.org/api/v2/package/<id>/<version>` with an `X-NuGet-ApiKey` header. This is the same call `dotnet nuget delete` makes, and on nuget.org it unlists the version rather than deleting it.
- There's a half-second pause between calls to stay well within nuget.org's rate limits.
