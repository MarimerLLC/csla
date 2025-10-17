Prerequisites
-------------
You must have:

1. PC
 1. Windows 11 or 10
 2. .NET 6 SDK
 2. .NET 7 SDK
 2. .NET 8 SDK
 3. Visual Studio 2022 with the latest updates
  1. Maui
  1. UWP SDK
  1. Blazor templates

Semantic Versioning
-------------------
CSLA .NET, starting with version 4.9.0, follows the [semantic versioning (semver)](https://semver.org/) guidelines. Version numbers should follow the semver guidance from that point forward.

NuGet release
-------------
1. Pull the latest code from MarimerLLC/csla
1. Open the csla.build.sln
   1. Update version numbers
      1. `cd /Source`
      1. Edit `Directory.Build.props` and update the version number
      1. `grep -rl --include=*.cs --include=*.csproj --include=*.Build.props '7\.0\.2' | xargs sed -i 's/7\.0\.2/8.0.0/g'`
         1. Adjust the version numbers to match current versions
   1. Build the solution in Release mode; Any CPU
1. Do NuGet release
   1. Open a powershell window
   1. Run the `nuget/Build All.ps1` script (add /prerelease:yymmddxx for test release)
   1. Make sure you have Rocky's NuGet key installed (see Nuget.org)
   1. Run the `nuget/Push All.ps1` script

Finalize Release
----------------
1. Update GitHub
   1. Update [releasenotes.md](https://github.com/MarimerLLC/csla/blob/master/releasenotes.md)
   1. Commit all changes to git
   1. Create PR 
   1. Accept PR
1. In the GitHub releases web page create the release
   1. Create a new release at HEAD using the version number (such as v7.0.2)
   1. Name the release like "Version 7.0.2 Release"
   1. Mark the release as pre-release or release
