Prerequisites
-------------
You must have:

1. PC
 1. Windows 10
 1. Visual Studio 2015 with the latest updates
  1. Xamarin
  1. UWP SDK
 1. Sandcastle installed

NuGet release
-------------
1. Pull the latest code from MarimerLLC/csla
1. Open the csla.build.sln
 1. Do a solution search/replace of the previous version number to the new version number
 1. Build the solution in Release mode; Any CPU
1. Do NuGet release
 1. Open a powershell window
 1. Run the `nuget\Build All.ps1` script (add /prerelease:Beta-yymmddxx for beta release)
 1. Make sure you have Rocky's NuGet key installed (see Nuget.org)
 1. Run the `nuget\Push All.ps1` script

Finalize Release
----------------
1. Update GitHub
 1. Commit all changes to git
 1. Create PR 
 1. Accept PR
1. In the GitHub releases web page create the release
 1. Create a new tag using the version number (such as v4.6.400)
 1. Attach the release to the tag
 1. Mark the release as pre-release or release
