# Create a CSLA .NET release

CSLA .NET packages are built and published to nuget.org by GitHub Actions. You start a release by pushing a branch with a specific name. You don't build or push packages from your own machine.

## How it works

| Push a branch named | Workflow | Builds | Publishes |
|---|---|---|---|
| `release/vX.Y.Z` | `.github/workflows/release.yaml` | `Source/csla.build.sln` | Every core package (`Csla`, `Csla.AspNetCore`, `Csla.Blazor`, `Csla.Testing`, the channels, the generators, and so on) |
| `release-maui/vX.Y.Z` | `.github/workflows/release-maui.yaml` | `Source/csla.maui.build.sln` | `Csla.Maui` only |

Both workflows:

* Run in the `nuget-release` GitHub environment. That environment only accepts `release/*` and `release-maui/*` branches, and a maintainer has to approve each run before it starts.
* Use [NuGet trusted publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing). The workflow trades its GitHub identity token for a nuget.org API key that lasts one hour, so no API key is stored in the repository or its secrets. The trusted publishing policy on nuget.org (owner `rockfordlhotka`) names these workflow files, so renaming a workflow breaks publishing until the policy is updated.
* Build with `-p:PublicRelease=true`, so the package version is exactly the one in `Source/version.json`, without a git commit suffix.
* Push with `--skip-duplicate`, so re-running a release doesn't fail on packages that are already on nuget.org.

`Csla.Maui` is released separately because it builds against the **published** `Csla` package from nuget.org, not a project reference. The core release has to be on nuget.org before the MAUI release can build.

## Which branch to release from

| Version | Source branch |
|---|---|
| Current major version in development (CSLA 11) | `main` |
| Earlier major version still maintained (CSLA 10) | `v10.x` |

Each source branch has its own `Source/version.json` and its own `releasenotes.md`. The examples below use `v10.x` and 10.2.0; substitute the right branch and version.

## Semantic versioning

CSLA .NET, starting with version 4.9.0, follows the [semantic versioning (semver)](https://semver.org/) guidelines.

* A stable release is `X.Y.Z`, such as `10.2.0`.
* A prerelease uses a dotted suffix, such as `10.2.0-beta.1` or `11.0.0-alpha.1`. nuget.org lists it as a prerelease.

## Release steps

### 1. Update the release notes

Make sure `releasenotes.md` on the source branch describes the release: highlights, changes by category, breaking changes, and contributors. Compare the commits since the previous release with the notes:

```bash
git log --oneline v10.1.0..origin/v10.x
```

Merge any updates through a PR.

### 2. Set the version

1. Create a branch from the source branch.
1. Change `version` in `Source/version.json`, for example from `10.2.0-beta.1` to `10.2.0`.
1. Open a PR into the source branch and merge it.

Don't change the `Csla` package version in `Source/Csla.Xaml.Maui/Csla.Xaml.Maui.csproj` yet. That version has to exist on nuget.org first (see step 5).

### 3. Publish the core packages

Push the source branch to a `release/` branch named after the version:

```bash
git fetch origin
git push origin origin/v10.x:refs/heads/release/v10.2.0
```

Then:

1. Open the **release** workflow run in the repository's **Actions** tab.
1. Review and approve the deployment to `nuget-release`.
1. Wait for the run to finish (about 10 to 15 minutes). The **Push NuGet packages** step should show `Your package was pushed.` for each package.

### 4. Wait for nuget.org to index the packages

After a push, nuget.org validates and indexes the packages before they can be downloaded. This usually takes 5 to 30 minutes. Check for the new version with:

```bash
curl -s https://api.nuget.org/v3-flatcontainer/csla/index.json
```

### 5. Point Csla.Maui and the samples at the released version

Once `Csla` X.Y.Z shows up on nuget.org:

1. Create a branch from the source branch.
1. Change the `Csla` package reference in `Source/Csla.Xaml.Maui/Csla.Xaml.Maui.csproj` to the new version.
1. Change the CSLA package versions in `Samples/` as well (the `CslaVersion` property in each `Directory.Packages.props` and the `PackageReference` versions in the sample projects). `git grep` for the old version to find them all.
1. Open a PR into the source branch. Because it changes `Csla.Xaml.Maui`, CI runs the **Build MAUI** job, which restores the new `Csla` package from nuget.org. Merge once it passes.

If you open this PR before nuget.org has indexed the package, the MAUI build fails because it can't restore `Csla`.

### 6. Publish Csla.Maui

```bash
git fetch origin
git push origin origin/v10.x:refs/heads/release-maui/v10.2.0
```

Approve the **release-maui** run as in step 3 and wait for `Csla.Maui.X.Y.Z.nupkg` to be pushed.

### 7. Create the GitHub release

1. On the [releases page](https://github.com/MarimerLLC/csla/releases), create a new release.
1. Create a new tag `vX.Y.Z` (such as `v10.2.0`) on the commit that was released, which is the head of the `release/vX.Y.Z` branch.
1. Name the release like "Version 10.2.0 release".
1. Use the release's section of `releasenotes.md` as the description.
1. Mark it as a pre-release for a prerelease version. For a stable release, set it as the latest release only if it's the highest version (a 10.x release made after 11.0.0 ships is not the latest).

Or with the GitHub CLI:

```bash
gh release create v10.2.0 --target <full commit sha> --title "Version 10.2.0 release" --notes-file notes.md --latest
```

`--target` needs a branch name or a full 40-character commit SHA.

The tag also makes the compare links in the release notes (such as `compare/v10.1.0...v10.2.0`) work.

### 8. Unlist superseded prereleases

After a stable release, unlist the prereleases it replaces (for example `10.2.0-beta.1` once `10.2.0` ships) with `Support/delist-prerelease-packages.py`. Run a dry run first:

```bash
cd Support
python delist-prerelease-packages.py
NUGET_API_KEY=<key> python delist-prerelease-packages.py --apply
```

The workflow's trusted publishing key can't be used for this. Create a short-lived nuget.org API key with only the **Unlist package** scope and delete it afterward. Wait until `Csla.Maui` is indexed too, so its prerelease is included. See [delist-prerelease-packages.md](../../Support/delist-prerelease-packages.md) for details.

### 9. Clean up

* Keep the `release/vX.Y.Z` and `release-maui/vX.Y.Z` branches as a record of what was published.
* Delete the PR branches from steps 1, 2 and 5 once their PRs are merged.

## Prereleases

A prerelease (alpha, beta, or release candidate) follows the same steps with a prerelease version, such as `release/v10.2.0-beta.1`. Skip step 8, and mark the GitHub release as a pre-release.

## If a release fails

* **The run never starts or waits forever:** it's waiting for approval in the `nuget-release` environment. Approve it in the run's page.
* **NuGet login fails:** the trusted publishing policy on nuget.org has to match this repository, the workflow file name, and the `nuget-release` environment. Check the policy under the `rockfordlhotka` account on nuget.org.
* **The MAUI build can't restore `Csla`:** the core release isn't indexed on nuget.org yet, or `Csla.Xaml.Maui.csproj` still references the wrong version.
* **Some packages pushed and others didn't:** fix the problem and push the release branch again (or re-run the workflow). `--skip-duplicate` skips the packages that are already on nuget.org.

Published packages can't be deleted from nuget.org, only unlisted. If a bad version ships, release a new patch version.

`Support/push-nuget-packages.sh` pushes the packages in `bin/packages` with an API key from your machine. It's only a manual fallback for when the workflows can't be used.
