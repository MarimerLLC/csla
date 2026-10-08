# CSLA 11 releases

CSLA 11 is the next major version of CSLA .NET, adding support for .NET 11 and removing support for .NET 8 and .NET 9. It is currently in development on the `main` branch.

Release notes for CSLA 10 are maintained on the [v10.x branch](https://github.com/MarimerLLC/csla/blob/v10.x/releasenotes.md), which is the maintenance branch for version 10.

## CSLA .NET version 11.0.0 release

CSLA 11 includes all of the changes released in CSLA 10.2.0. The full list of changes made since that release can be found in the [GitHub compare view](https://github.com/MarimerLLC/csla/compare/v10.2.0...main).

### Highlights

**Platform Updates** ([#4699](https://github.com/MarimerLLC/csla/issues/4699), [#4348](https://github.com/MarimerLLC/csla/issues/4348), [#4581](https://github.com/MarimerLLC/csla/issues/4581), [#4922](https://github.com/MarimerLLC/csla/pull/4922))

* Add support for .NET 11
* Remove support for .NET 8 and .NET 9, which Microsoft stops supporting in November 2026
* `netstandard2.0` and .NET Framework 4.6.2 through 4.8 targets are unchanged
* `Csla.Maui` targets `net11.0-android`, `net11.0-ios`, `net11.0-maccatalyst`, and `net11.0-windows` alongside the .NET 10 targets

### Changes

**Build and Release**

* [#4917](https://github.com/MarimerLLC/csla/issues/4917) Set the `main` version to 11.0.0 and start the CSLA 11 release notes ([#4918](https://github.com/MarimerLLC/csla/pull/4918))

### Breaking Changes

* CSLA 11 no longer targets .NET 8 or .NET 9. Applications must target .NET 10 or later (or continue to use .NET Framework 4.6.2 through 4.8, or a `netstandard2.0`-compatible runtime).
* On .NET 11, Android apps using `Csla.Maui` require Android API level 24 or later, because .NET 11 for Android requires it. The `net10.0-android` target still supports API level 21.

### Supported Platforms

* .NET 10 and 11
* .NET Framework 4.6.2 through 4.8
* Blazor (Server, WebAssembly, Auto)
* MAUI
* ASP.NET Core MVC, Razor Pages, Web API
* Windows Forms, WPF
* Avalonia
* ASP.NET MVC 5, WebForms

Also expected to work on:

* Uno Platform

### Contributors

* [@rockfordlhotka](https://github.com/rockfordlhotka)

Thank you all for your contributions!
