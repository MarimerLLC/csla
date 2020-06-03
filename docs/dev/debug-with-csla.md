# Debugging with CSLA Code

When working on CSLA it is often desirable to build/run a sample app with the actual CSLA code. Also it is pretty common to want to build and reference non-public NuGet packages for CSLA from a local/private test app.

## Build/Run with CSLA Code

To build/run with CSLA code do this:

1. Clone/pull the latest CSLA repo from GitHub
1. Create your sample/test app
1. In your sample/test solution, add the following existing projects _in this order_:
   1. `Csla.Shared` (shared project)
   1. `Csla.NetStandard2.0` (concrete project that relies on shared project)
   1. Other `.Shared` projects necessary for any concrete projects (such as `Csla.AspNetCore.Shared' and `Csla.Web.Mvc.Shared`)
   1. Other concrete projects (such as `Csla.AspNetCore.NetCore3.1`)
1. In your sample/test projects add project references to the _concrete_ CSLA projects necessary (such as `Csla.NetStandard2.0` and `Csla.AspNetCore.NetCore3.1`)
1. You can now build and run the solution, including all the CSLA code as well as your sample/test code

## Create and Use Local NuGet Packages

1. Clone/pull the latest CSLA repo from GitHub
1. Open the `csla.build` solution and rebuild all
1. Open a PowerShell window and navigate to `\csla\nuget`
1. Execute `build all.ps1 /prerelease:yymmddxx` where yy=year, mm=month, dd=day, xx=ascending number (01, 02, etc)
1. Execute `consolidatepackages.ps1` to copy packages to `\Packages` (at peer level with `\csla`)
1. In Visual Studio add a new NuGet package location corresponding to your `\Packages` directory location; I name mine `csladev`
1. You can now reference NuGet packages from nuget.org or from your new csladev location
