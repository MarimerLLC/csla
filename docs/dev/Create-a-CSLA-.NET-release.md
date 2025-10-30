# Create a CSLA .NET release

## Prerequisites

You must have:

1. Windows PC
  a. Windows 11
  a. .NET SDKs
     1. 8 SDK
     1. 9 SDK
     1. 10 SDK
  a. Visual Studio 2026 with the latest updates/toolsets
     1. Maui
     1. Blazor templates

## Semantic Versioning

CSLA .NET, starting with version 4.9.0, follows the [semantic versioning (semver)](https://semver.org/) guidelines. Version numbers should follow the semver guidance from that point forward.

## NuGet release

1. Pull the latest code from MarimerLLC/csla
1. Open the `Source/version.json` file and update the version number
1. ⚠️ Commit the change to git ⚠️
1. Do NuGet release
   1. Open a terminal window
   1. Change to the `csla/Source` folder
   1. Run `dotnet pack csla.build.sln`
   1. Make sure you have Rocky's NuGet API key (see Nuget.org)
   1. Change to the `csla/Support` folder
   1. Run the `csla/Support/push-nuget-packages.sh` bash script to push the packages

## Finalize Release

1. Update GitHub
   1. Update [releasenotes.md](https://github.com/MarimerLLC/csla/blob/master/releasenotes.md)
   1. Commit all changes to git
   1. Create PR
   1. Accept PR
1. In the GitHub releases web page create the release
   1. Create a new release at HEAD using the version number (such as v10.0.2)
   1. Name the release like "Version 10.0.2 Release"
   1. Mark the release as pre-release or release
