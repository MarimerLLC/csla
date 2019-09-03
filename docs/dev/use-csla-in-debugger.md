# Using CSLA in a Local Project

For any production scenario you should reference CSLA using the published NuGet packages.

When developing CSLA itself though, it is often nice to use local NuGet packages, or even reference/debug the CSLA codebase itself.

## Using Local NuGet Packages

Here are the steps to create a local "prerelease" of CSLA NuGet packages you can use for local testing:

1. Open the `csla.build.sln` solution
1. Rebuild All
1. Open PowerShell
   1. Change directory to `csla/NuGet`
   1. `'.\Build All.ps1' /prerelease:yymmddnn`
      * yy is year, mm is month, dd is day, nn is a unique ascending number for the current day (01, 02, etc.)
   1. `.\consolidatepackages.bat`
      * This copies the packages to a `packages` directory at a _peer level_ with your `csla` directory
1. In Visual Studio add a NuGet package source that points to this new `packages` directory

You should now be able to use normal NuGet referencing to reference the packages from this NuGet source, so your code can leverage the unpublished CSLA NuGet packages you've created.

## Directly Debugging CSLA Code

When developing CSLA code it is often beneficial to walk through the CSLA code itself in the debugger.

The easiest way to do this is to create a new project of whatever type you want to use (web, Windows, Xamarin, etc.) and then direcly reference the _CSLA projects_ containing the code you want to debug.

Remember that CSLA code is typically in a Shared Project, with any per-platform projects pulling code from the Shared Project. This means that **order matters** when referencing the CSLA projects.

1. Reference the Shared Project first (such as `Csla.Shared.shproj`)
1. Then reference the per-platform project (such as `Csla.NetStandard2.0.csproj`)

Do NOT reference any NuGet CSLA packages. You are either referencing everything via project references or nothing via project references. This includes all the CSLA satellite projects too (Xaml, web, etc.).
